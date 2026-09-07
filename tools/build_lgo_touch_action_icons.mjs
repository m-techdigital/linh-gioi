import fs from 'node:fs';
import path from 'node:path';
import { createHash } from 'node:crypto';
import { createRequire } from 'node:module';
import { fileURLToPath } from 'node:url';

const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const modules = path.join(root, 'build/toolchains/lgo-icons/node_modules');
const require = createRequire(import.meta.url);
// npm install --prefix build/toolchains/lgo-icons --no-audit --no-fund lucide-static@1.21.0 @resvg/resvg-js@2.6.2
if (JSON.parse(fs.readFileSync(path.join(modules, '@resvg/resvg-js/package.json'))).version !== '2.6.2')
  throw new Error('Expected @resvg/resvg-js@2.6.2');
const { Resvg } = require(path.join(modules, '@resvg/resvg-js'));
const lucide = path.join(modules, 'lucide-static');
if (JSON.parse(fs.readFileSync(path.join(lucide, 'package.json'))).version !== '1.21.0')
  throw new Error('Expected lucide-static@1.21.0');
const output = path.join(root, 'client/Unity/Assets/Game/UI/Resources/LGOUI');
for (const [name, source] of Object.entries({ ActionTalk: 'message-circle-more', ActionTouch: 'hand', ActionComplete: 'check' })) {
  const glyph = fs.readFileSync(path.join(lucide, 'icons', source + '.svg'), 'utf8');
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="24" height="24" color="white">${glyph}</svg>`;
  const png = new Resvg(svg, { fitTo: { mode: 'width', value: 64 } }).render().asPng();
  if (png.length > 8192) throw new Error('Icon exceeds 8 KiB budget: ' + name);
  const file = path.join(output, name + '.png');
  fs.writeFileSync(file, png);
  if (!fs.existsSync(file + '.meta')) {
    const guid = createHash('md5').update('LGOUI/' + name).digest('hex');
    fs.writeFileSync(file + '.meta', `fileFormatVersion: 2
guid: ${guid}
TextureImporter:
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    enableMipMap: 0
    sRGBTexture: 1
  isReadable: 0
  maxTextureSize: 64
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  alphaIsTransparency: 1
  textureType: 0
  textureShape: 1
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 64
    textureCompression: 0
    overridden: 0
`);
  }
  console.log(name, '64x64', png.length, 'bytes', createHash('sha256').update(png).digest('hex'));
}
fs.copyFileSync(path.join(lucide, 'LICENSE'), path.join(output, 'Lucide-LICENSE.txt'));
