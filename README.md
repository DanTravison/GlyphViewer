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
to survey the various open source fonts

For example, some glyphs need to be centered on the staff lines using the glyph's height, 
while others need the baseline aligned to a staff line, space, or ledger line, and 
and a few needed custom logic.

Finally, while the project is currently tested only on Windows, my intent is to 
test on iOS, MacCatalyst, and Android to aid in my testing of the sheet music rendering
on those platforms.

The resulting Glyph and Metrics visualization:
![Glyph View](images/GlyphView.png)

## The project structure

## Views
* MainPage: The application's main page
  * Presents the font family list, GlyphsView, GlyphView and MetricsView 
* GlyphsView: The view of the glyphs rendered on a SkCanvasView
* GlyphView: The view of the selected glyph rendered on a SkCanvasView
* MetricsView: The view of the glyph properties.

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
  * NOTE: These are from [Character Map UWP](https://github.com/character-map-uwp/Character-Map-UWP) 

## Controls\Grid
A derived Maui grid that supports data binding.
The top portion of the Glyph metrics is displayed using this control.

## Controls\Slider
A simple slider that supports vertical and horizontal orientations.
I needed an alternative to a ScrollView so I can, essentially, virtualize
the SKCanvas instead of creating a canvas sufficient to display all items.

## Other
* ExtendedUnicode.txt: A text file containing links to the Unicode ranges on www.unicode.org
* ObjectModel/ObservableObject: A simple implementation of INotifyPropertyChanged

## Dependencies
* SkiaSharp.Views.Maui.Controls: 3.116.1
* Microsoft.Maui.Controls: 9.0.50
