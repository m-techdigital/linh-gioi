import unittest

import validate_lgo_owner_review_catalog as validator


class OwnerReviewCatalogValidatorTests(unittest.TestCase):
    def test_current_catalog_contains_visually_audited_source_pose_classes(self):
        self.assertEqual(validator.main(), 0)


if __name__ == "__main__":
    unittest.main()
