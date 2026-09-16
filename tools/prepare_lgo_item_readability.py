"""A fixed source-lighting pass for registered painted UI items, not runtime tint.

Apply to pinned full-precision sources, never cumulatively to an encoded atlas.
A whole reviewed set uses one curve. Alpha, coordinates and highlights stay exact.
This improves dark-surface readability; it does not validate motif or art direction.
"""
from PIL import Image

PROFILE = 'lgo-painted-item-shadow128-highlights-alpha-exact-v1'
SOURCE_SIZE = (384, 384)


def lift_shadows(source: Image.Image) -> Image.Image:
    """Lift painted shadows while leaving pixels with peak RGB >=128 unchanged.

    Below128, gain=1+0.6*(1-peak/128). This is monotonic, joins the unchanged
    highlights continuously and preserves channel ratios within integer rounding.
    No palette swap, geometry, matte, sharpening, outline or glow is introduced.
    """
    if source.mode != 'RGBA' or source.size != SOURCE_SIZE:
        raise ValueError('Readability input must be registered RGBA 384x384')
    gains = [1.0] + [1 + .6*(1-value/128) if value < 128 else 1.0 for value in range(1, 256)]
    result = source.copy()
    result.putdata([
        (round(r*gains[max(r,g,b)]), round(g*gains[max(r,g,b)]),
         round(b*gains[max(r,g,b)]), a) if a else (r,g,b,a)
        for r,g,b,a in source.getdata()
    ])
    return result
