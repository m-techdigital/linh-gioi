"""Exercise checkpoint safety with real isolated repositories and a local remote."""
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest


SCRIPT = Path(__file__).with_name('lgo_codex_git_checkpoint.sh')


class GitCheckpointTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.main = self.root / 'main'
        self.remote = self.root / 'remote.git'
        self.run_git(self.root, 'init', '--bare', str(self.remote))
        self.run_git(self.root, 'init', '-b', 'feature/2d', str(self.main))
        self.run_git(self.main, 'config', 'user.email', 'checkpoint@example.invalid')
        self.run_git(self.main, 'config', 'user.name', 'Checkpoint test')
        (self.main / 'tools').mkdir()
        shutil.copy2(SCRIPT, self.main / 'tools' / SCRIPT.name)
        (self.main / '.gitignore').write_text('build/\n')
        (self.main / 'README.md').write_text('baseline\n')
        (self.main / 'protocol').mkdir()
        (self.main / 'protocol' / 'frozen.txt').write_text('locked\n')
        self.run_git(self.main, 'add', '.')
        self.run_git(self.main, 'commit', '-m', 'baseline')
        self.run_git(self.main, 'remote', 'add', 'origin', str(self.remote))
        self.run_git(self.main, 'push', '-u', 'origin', 'feature/2d')
        self.work = self.root / 'isolated-worktree'
        self.run_git(self.main, 'worktree', 'add', '-b', 'codex/review', str(self.work), 'origin/feature/2d')
        status = self.work / 'build/codex-autopilot/status.json'
        status.parent.mkdir(parents=True)
        status.write_text(json.dumps({'status': 'CONTINUE', 'current_task': 'test checkpoint'}))

    def run_git(self, cwd, *args):
        return subprocess.run(['git', *args], cwd=cwd, check=True, capture_output=True, text=True).stdout.strip()

    def checkpoint(self, push=False):
        env = dict(os.environ, LGO_AUTOPILOT_PUSH='1' if push else '0')
        return subprocess.run(['bash', str(self.work / 'tools' / SCRIPT.name), 'test'],
                              cwd=self.root, env=env, capture_output=True, text=True)

    def tip(self):
        return self.run_git(self.work, 'rev-parse', 'HEAD')

    def remote_tip(self):
        return self.run_git(self.remote, 'rev-parse', 'refs/heads/feature/2d')

    def test_worktree_name_and_different_tracking_branch_are_supported(self):
        (self.work / 'README.md').write_text('coherent batch\n')
        result = self.checkpoint(push=True)
        self.assertEqual(0, result.returncode, result.stderr + result.stdout)
        self.assertEqual(self.tip(), self.remote_tip())
        self.assertEqual('baseline\n', (self.main / 'README.md').read_text())
        self.assertIn('LGO_GIT_CHECKPOINT_COMMITTED', result.stdout)

    def test_clean_ahead_branch_can_push_existing_checkpoint(self):
        (self.work / 'README.md').write_text('already committed batch\n')
        self.run_git(self.work, 'commit', '-am', 'reviewed checkpoint')
        before = self.tip()
        result = self.checkpoint(push=True)
        self.assertEqual(0, result.returncode, result.stderr + result.stdout)
        self.assertEqual(before, self.tip())
        self.assertEqual(before, self.remote_tip())

    def test_unstaged_deletion_is_committed_once_and_pushed(self):
        (self.work / 'README.md').unlink()
        result = self.checkpoint(push=True)
        self.assertEqual(0, result.returncode, result.stderr + result.stdout)
        self.assertEqual(self.tip(), self.remote_tip())
        self.assertEqual('', self.run_git(self.work, 'ls-files', 'README.md'))
        self.assertEqual('', self.run_git(self.work, 'status', '--porcelain'))
        self.assertEqual('baseline\n', (self.main / 'README.md').read_text())

    def test_cached_only_changes_are_committed(self):
        (self.work / 'README.md').write_text('staged batch\n')
        self.run_git(self.work, 'add', 'README.md')
        before = self.tip()
        result = self.checkpoint()
        self.assertEqual(0, result.returncode, result.stderr + result.stdout)
        self.assertNotEqual(before, self.tip())

    def test_frozen_changes_are_blocked_without_unstaging(self):
        (self.work / 'protocol/frozen.txt').write_text('unauthorized\n')
        self.run_git(self.work, 'add', 'protocol/frozen.txt')
        before = self.tip()
        index_before = self.run_git(self.work, 'write-tree')
        result = self.checkpoint(push=True)
        self.assertEqual(3, result.returncode, result.stderr + result.stdout)
        self.assertEqual(before, self.tip())
        self.assertEqual(index_before, self.run_git(self.work, 'write-tree'))
        self.assertEqual(before, self.remote_tip())

    def test_untracked_frozen_file_blocks_before_staging_other_changes(self):
        (self.work / 'protocol/new.txt').write_text('unauthorized\n')
        (self.work / 'README.md').write_text('allowed\n')
        result = self.checkpoint()
        self.assertEqual(3, result.returncode, result.stderr + result.stdout)
        self.assertEqual('', self.run_git(self.work, 'diff', '--cached', '--name-only'))

    def test_clean_branch_cannot_push_frozen_commit(self):
        (self.work / 'protocol/frozen.txt').write_text('unauthorized\n')
        self.run_git(self.work, 'commit', '-am', 'frozen edit')
        before = self.remote_tip()
        result = self.checkpoint(push=True)
        self.assertEqual(3, result.returncode, result.stderr + result.stdout)
        self.assertEqual(before, self.remote_tip())

    def test_push_requires_configured_upstream(self):
        self.run_git(self.work, 'branch', '--unset-upstream')
        result = self.checkpoint(push=True)
        self.assertEqual(3, result.returncode, result.stderr + result.stdout)
        self.assertIn('upstream', result.stderr)

    def test_diverged_remote_is_not_overwritten(self):
        (self.main / 'README.md').write_text('remote progress\n')
        self.run_git(self.main, 'commit', '-am', 'remote checkpoint')
        self.run_git(self.main, 'push', 'origin', 'feature/2d')
        before = self.remote_tip()
        (self.work / 'README.md').write_text('local progress\n')
        self.run_git(self.work, 'commit', '-am', 'local checkpoint')
        result = self.checkpoint(push=True)
        self.assertEqual(3, result.returncode, result.stderr + result.stdout)
        self.assertEqual(before, self.remote_tip())


if __name__ == '__main__':
    unittest.main()
