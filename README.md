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
  * Display the text code for each glyph.
  * Display the glyph name when available.

# The Project Structure

## Views
* MainPage: The application's main page
  * Presents the HeaderView, FontFamiliesView, GlyphsView, GlyphView and MetricsView 
* GlyphsView: The view of the glyphs in a typeface rendered on a SkCanvasView
* GlyphView: The view of the selected glyph rendered on a SkCanvasView
* MetricsView: The view of the glyph properties.
* FontFamiliesView: The list of available fonts (typefaces) grouped by the first letter of the typeface name..
* FamilyGroupPicker: A jump list to select a font family group.
* HeaderView: The header for the main page.

## Views/Renderers
* IGlyphRow: Interface for the GlyphRow and HeaderRow classes
* GlyphRowBase: Base class for header and glyph rows
* HeaderRow: A row in the GlyphsView for each Unicode range.
* GlyphRow: A row in the Glyphs
* DrawingContext: The context for drawing the glyphs and rows
  * Contains the various fonts, colors, and layout metrics used by the renderers
  * Called by various GlyphsView properties to synchronize changes needed for rendering.

## Text
Contains the various Glyph classes:
* Glyph: The basic Glyph class
* SKTextMetrics: Provides general text measurement metrics.
* NamedValue: A simple Named/Value base class used for Glyph and Font metrics property display.
* GlyphMetrics: Provides text metrics for the glyph.
* GlyphMetricsProperties: A NamedValue collection for displaying glyph metrics
* Fonts: A couple of extension methods.
* TextUtilities: Used by Grid to measure column widths.
* FontFamilyGroupCollection: A collection of FontFamilyGroup.
* FontFamilyGroup: A collection of font families in a given group.
* FontMetricsProperties: A A NamedValue collection for displaying displaying font metrics
* Fonts.cs: A set of extension methods
  * ToStyleFont - converts FontAttributes to SKFontStyle
  * GetFontFamilies - enumerate all font families visible to SkiSharp
  * ToPixels - converts a point size to pixels using 96/72
	
Fonts.cs Experimental Extension Methods:
  * ScalePoints - Scales font points to pixels at the current display density 
  * DrawText - Wraps Canvas.DrawText by scaling the SKFont.Size using ScalePoints
  * Measure - wraps SKFont.MeasureText by scaling the SKFont.Size using ScalePoints 

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

## Controls\JumpList
Provide a control template for a jump list.
This is used to select a font family group in the FontFamiliesView and a unicode range in GlyphsView. 

## ObjectModel
* ObservableObject: An implementation of INotifyPropertyChanged with SetProperty overloads.
  * SetProperty takes an instance of a PropertyChangedEventArgs to allow derived class to statically define 
    PropertyChangedEventArgs instead of creating instances on each call. 
* Command: An implementation of ICommand with IsEnabled for controlling CanExecute.

## Other
* ExtendedUnicode.txt: A text file containing links to the Unicode ranges on www.unicode.org
* Images/GlyphView.png: A screen shot of the GlyphView

## Dependencies
* SkiaSharp.Views.Maui.Controls: 3.116.1
* Microsoft.Maui.Controls: 9.0.50
