# GlyphViewer
Provides an experimental view of individual glyphs in a typeface 
displaying the Glyph and associated metrics using SkiaSharp.

The application enumerates all fonts available via SkiaSharp and, for each,
lists the available glyphs. When a glyph is selected, a zoomed view of the 
glyph is displayed with the ascent, baseline, left and right edges displayed
via a set of dotted lines.

The need for this came out of another project where I display a piano keyboard
and associated sheet music.  

I had been using [Character Map UWP](https://github.com/character-map-uwp/Character-Map-UWP)
to survey the various open source fonts but found I needed to visualize the metrics (e.g. baseline,
ascent, descent, etc.) for the glyphs in a typeface to determine how to align the glyphs on 
the staff lines.

For example, some glyphs need to be centered on the staff lines using the glyph's height, 
while others need the baseline aligned to a staff line, space, or ledger line, and 
and a few needed custom logic.  Finally, some annotations need to be aligned relative
to the staff itself or notes, such as articulations, accidentals, tempo and dynamic markings.

<p align="center">The resulting Glyph and Metrics visualization</p>
<p align="center">
<img src="https://github.com/DanTravison/GlyphViewer/blob/main/Images/GlyphView.png" width="25%" height="25%">
</p>

# Status
* The project is a work in progress.
  * The logic for enumerating glyphs in a typeface is a work in progress. 
  * The range of Unicode characters is currently 0x0000-0xFFFF.
* Testing is manual on Windows.
  * Testing on iOS, MacCatalyst, and Android is planned.
* The GlyphsView is still rather minimal. I'm considering the following changes:
  * Group glyphs by Unicode range and jump list support.
  * Display the text code for each glyph.

# The project structure

## Views
* MainPage: The application's main page
  * Presents the HeaderView, FontFamiliesView, GlyphsView, GlyphView and MetricsView 
* GlyphsView: The view of the glyphs in a typeface rendered on a SkCanvasView
* GlyphView: The view of the selected glyph rendered on a SkCanvasView
* MetricsView: The view of the glyph properties.
* FontFamiliesView: The list of available fonts (typefaces) grouped by the first letter of the typeface name..
* FamilyGroupPicker: A jump list to select a font family group.
* HeaderView: The header for the main page.

## Text
Contains the various Glyph classes:
* Glyph: The basic Glyph class
* GlyphMetrics: Metrics for the glyph derived from measuring the associated text.
* GlyphProperty: A simple name/value class for a given property on the Glyph and GlyphMetrics
* GlyphMetricsProperties: A simple collection of GlyphProperty
* Fonts: A couple of extension methods.
* TextUtilities: Used by Grid to measure column widths.
* FontFamilyCollection: Grouped by the first letter of the family name
* FontFamilyGroup: A collection of font families in a given group.

## Text\Unicode
* Range: A Unicode range
* Ranges: the set of Unicode ranges in 0x0000-0xFFFF
* Extended: The set of Unicode ranges from 0x10000 through 0x100000
  * NOTE: These are extracted from [Character Map UWP](https://github.com/character-map-uwp/Character-Map-UWP) 
  * See the associated [LICENSE](https://github.com/character-map-uwp/Character-Map-UWP/blob/master/LICENSE) 

## Controls\Grid
A derived Maui grid that supports data binding.
This is used to display the list of glyph metric properties.
Most of the capabilities for column sizing are not being used.

## Controls\Slider
A simple slider that supports vertical and horizontal orientations.
This is used to scroll the GlyphsView content versus scrolling a large SKCanvasView.

## Other
* ExtendedUnicode.txt: A text file containing links to the Unicode ranges on www.unicode.org
* ObjectModel/ObservableObject: An implementation of INotifyPropertyChanged

## Dependencies
* SkiaSharp.Views.Maui.Controls: 3.116.1
* Microsoft.Maui.Controls: 9.0.50
