# GlyphViewer

Provides an experimental view of individual glyphs in a typeface 
displaying the Glyph and associated metrics using SkiaSharp.

The application enumerates all fonts available via SkiaSharp and, for each,
lists the available glyphs. When a glyph is selected, a zoomed view of the 
glyph is displayed with the ascent, baseline, left and right edges displayed
via a set of dotted lines.  Additionally, both the glyph and font metrics are displayed.

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
* The basical functionality is complete.
  * The range of Unicode characters is currently limited to 0x0000-0xFFFF.
* Testing is manual on Windows.
  * Testing on iOS, MacCatalyst, and Android is planned.
* Testing on Android tablets is in progress.
  * Converting MauiFont to embedded resource was needed to use SkiaSharp on Android
  * See FontFamily, FontLoader and FontResource below.
  * Currently tracking [issue 3283](https://github.com/mono/SkiaSharp/issues/3283)
    * SKTypeface.FromFamilyName is falling back to serif and sans-serif on Android for many installed fonts. 
    * The tool is useful for embedded or file-system files but not for installed fonts.
* The GlyphsView is still in progress. I'm considering the following enhancements:
  * Display the text code for each glyph in the font's glyph list.
* Currently tracking [issue 3239](https://github.com/mono/SkiaSharp/issues/3239) in SkiaSharp
  * There is a workaround in the [GLyphView](https://github.com/DanTravison/GlyphViewer/issues/23)
  * There is another workaround in [SkLabel](https://github.com/DanTravison/GlyphViewer/issues/25)
* Currently tracking [issue 29284](https://github.com/dotnet/maui/issues/29284)
  * CollectionView should support CollectionChanged events on a custom collection class
  * [Bookmarks](https://github.com/DanTravison/GlyphViewer/blob/main/Settings/Bookmarks.cs) is using a temporary workaround based on ReadOnlyCollection\<T\>.
* Currently tracking [issue 29484](https://github.com/dotnet/maui/issues/29484)
  * CollectionView Selected state does not work on the selected item when combined with PointerOver.
* Currently tracking [issue 18700](https://github.com/dotnet/maui/issues/18700)
  * [Windows] CollectionView GroupHeader doesn't fill space horizontaly
* Currently tracking [issue 19639](https://github.com/dotnet/maui/issues/19639)
  * On android, collectionview items cannot be selected.
  * No resolution is expected.
* Currently tracking [issue 3280](https://github.com/mono/SkiaSharp/discussions/3280)
  * Why doesn't SKTypeface.FromStream support SKFontStyle?
  * No resolution is forthcoming.
* The repo file structure will change to support the unit test assembly.
  * Move GlyphViewer into a child directory.
  * Merge the unit tests assembly into the repo. 
* Experimental: Load fonts from the local file system.
  * This supports displaying fonts that are not installed on the system/not visible in SkiaSharp.
  * The SettingsPage has a FileFonts section that allows loading fonts from the local file system.
  * The fonts are loaded into the FileFonts setting and displayed in the FontFamiliesView.
  * All font loading is now through FontFamily and FileFontFamily.
  * File-based fonts are displayed using the file name versus the font family name to avoid confusion. 
  Experimental: Defining a font as an embedded resource versus a MauiFont.
  * MauiFont resources are not visible to SkiaSharp.
  * The change converts MauiFont to an embedded resource font using FontResource.
  * FontFamily uses FontLoader and FontResource to load the embedded resource font for SkiaSharp.
  * NOTE: SkiaSharp does not support setting SKFontStyle for embedded resource fonts or fonts loaded from the file system.
  * ISSUE: Using an embedded resource font in Maui on Android works but causes multiple Java.Lang.RuntimeException log entries 

# The Project Structure

## ViewModels
* MainViewModel: the view model for the main page and various views.
* FontGlyphsViewModel: provides the view model for FontGlyphsView.
* PageNavigator: provides open/close semantics for a specific ContentPage.
  * Defines an OpenCommand to open a page and a CloseCommand to navigate back.
  * Creates the ContentPage and optionally sets the BindingContext on open.
  * Pops the page from the navigation stack on close and clears the BindingContext when the page is unloaded.  
  * IsModel indicates whether the page should be modal.
* MetricsViewModel: the view model for the fonts and glyph metrics.
  * Provides the property for the current Glyph. 
  * Provides a list of glyph metrics properties for the selected glyph.
  * Provides a list of font metrics properties for the selected typeface.
* SearchViewModel: The view model for search in the GlyphsView.
  * Provides the property for the current search string.
  * Provides a list of glyphs matching the search string.
  * Provides commands to search and to show/hide search results.
  * SelectedItem property is bound to the selected glyph SearchView.
	* Updates MetricsModel.Glyph when the selected glyph changes.
* BookmarkCommand: A command to add and remove font families from bookmarks.
  * Used by MainViewModel.BoomkarkCommand. 
* SettingsViewModel: The view model for the SettingsPage.
  * Provides the property for the current UserSettings.
  * Provides commands to load and unload file system-based fonts

## Views
* MainPage: The application's main page
  * Presents the HeaderView, FontFamiliesView, FontGlyphsView, GlyphMetricsView .
   * HeaderView: The header for the main page.
* FontGlyphsView
  * Encapsulates the GlyphsView, SearchView, slider, jumplist.
  * GlyphsView: The view of the glyphs in a typeface rendered on a SkCanvasView.
  * SearchView: Provides the edit field and associated search results in the FontGlyphsView.
    * Search results expand down from the search edit field. 
* GlyphMetricsView: Encaspulates the GlyphView and MetricsView.
  * GlyphView: The view of the current glyph rendered on a SkCanvasView
  * MetricsView: The view of the current glyph and font family properties.
* FontFamiliesView: The list of available fonts (typefaces) grouped by the first letter of the typeface name..
  * FamilyGroupPicker: A jump list to select a font family group.
* SettingsPage: A page for user configurable settings.
* FileFontsView: a view for loading/unloading fonts from the local file system.
  * Provides a list of fonts in the local file system.
  * Provides a button to load the selected font into the FileFonts setting.
  * Provides a button to unload the selected font from the FileFonts setting.
  * Presented in the SettingsPage as a child view. 

## Views/Renderers
* GlyphsViewRenderer - provides layout, rendering and hit testing for GlyphsView
* IGlyphRow: Interface for the GlyphRow and HeaderRow classes
* GlyphRowBase: Base class for header and glyph rows
* GlyphRow: A row in the Glyphs
* GlyphRowRenderer: Provides layout, rendering and it testing for GlypRow.
* GlyphRenderer: Renders a glyph in the GlyphRow.
* HeaderRow: A row in the GlyphsView for each Unicode range in the font.
* CellLayoutStyle: Describes the layout for sizing cells in a GlyphRow.
  * This is considered experimental to allow visualizing various cell sizing models.
  * The default sets the cell to the tallest and widest glyph in the font.
  * Cell and Row height can be set to the default or to the tallest glyph in a GlyphRow.
  * Cell width can be set to the default, the width of the widest glyph in a row or the individual glyph widths.  
* SkSpacing: Defines the spacing around a glyph in the GlyphRow.
  * This is a simplfied Thickness class since only Horizontal and Vertical values are needed. 
* DrawingContext: The context for drawing the glyphs and rows
  * Contains the various fonts, colors, and layout metrics used by the renderers
  * Listens for changes on GlyphsView to synchronizing drawing properties.

## Settings
* ISettingSerializer: defines the JSON serialization contract.
* ISetting: an ISettingProperty with a Parent property.
* Setting: Implements ISetting.
  * Derives from SettingPropertyCollection
  * Implements ISettingSerializer via the base SettingPropertyCollection.
* UserSettings - encapsulates all settings.
  * Provides properties for concrete Settings. 
  * Provides Load and Save method using UserSettingsJsonConverter. 
  * Settings are serialized to AppDataDirectory/settings.json
* FontFamiliesSettings: An ISetting base class containing a collection of font families. 
* Boomarks: A FontFamiliesSettings containing the set of bookmarked font family names.
* FileFonts: A FontFamiliesSettings containing the set of fonts from the local file system.
* FontSetting: an abstract base class for font settings.
* ItemFontSetting: Provides the font settings rows in the GlyphsView
* ItemHeaderFontSetting: Provides the font settings for header rows in the GlyphsView
* TitleFontSetting: Provides the font settings for the main page's title view.
* GlyphSetting: Provides general Glyph properties and constants
  * Width - the width of the GlyphView pane. 
  * CellLayout - the CellLayoutStyle for GlyphRow cells. 
* CellLayoutStyle: Provides the layout style for the cells in a GlyphRow.
* CellLayoutModel: Provides a bindable object for editing CellLayoutStyle properties.
  * WidthOptions: the list of CellLayoutOption for editing the cell width
  * HeightOptions: The list of CellLayoutOption instances for editing the cell height.  
  * Both are intended to support ItemsSource binding in PageSettings. 
* SettingDataTemplateSelector - Provides the template selector for the SettingsPage.
* Constants and Defaults
  * All constants and defaults are defined in the associated Setting class. 
  * Various View BindingProperty definitions refer to the associated Setting property for default values.
  * When a property has a defined range, the setting will declare Minimum/Maximum/Increment constants.
  * BindableProperty coerce delegates will clamp the value to the constants intead of failing.
  * The sliders presented in SettingsPage will also use these values to define the range and increment. 

## Settings/Properties
* SettingPropertyCollection: provides an ISettingProperty collection.
  * Implements ISettingSerializer for both ISetting and ISettingProperty. 
* ISettingProperty: contract for named setting property
  * Derives from ISettingSerializer 
* SettingProperty\<T\>: implements ISettingProperty and ISettingSerializer
* DoubleProperty: provides an ISettingProperty\<double\>
* StringProperty: provides an ISettingProperty\<string\>
* FontFamilyProperty: a StringProperty for a font family name
* FontSizeProperty: a DoubleProperty for a font size.
* EnumProperty: An ISettingProperty for Enum types.
* FontAttributesProperty: an ISettingProperty\<FontAttributes\>
* CellLayoutProperty: An ISettingProperty\<CellLayoutStyle\>
  * Defines CellLayoutModel.WidthOptions and CellLayoutModel.HeightOptions properties
  * Consumed by SettingsPage for editing CellLayoutStyle properties. 

## Resource clases for embedded resource fonts
* FontResource:
  * Provides a class for encapsulating for a font defined as an embedded resource. 
  * Used in-lieu of MauiFont to enable SkiaSharp using the embedded resource font.
  * Provides a GetTypeface() method that loads the embedded resource font and caches the SKTypeface.
* FontLoader: A static class for loading embedded resoure fonts into Maui.
  * Provides a Resolve method to resolve font 'names' to embedded resource names.
  * Provides a Load(FontResource) method to resolve a name to a FontResource.
  * The name can be the FontResource.Name, FontResource.Alias, or FontResource.ResourceName.
  
## Text
Contains the various Glyph classes:
* Glyph: The basic Glyph class
* SKTextMetrics: Provides general text measurement metrics.
* NamedValue: A simple Named/Value base class used for Glyph and Font metrics property display.
* GlyphMetrics: Provides text metrics for the glyph.
* GlyphMetricsProperties: A NamedValue collection for displaying glyph metrics.
* FontFamily: Represents a font registered in the system.
  * GetTypeface: Creates an SKTypeface for the font.
  * CreateFont: Creates an SKFont for the font. 
  * Integrates with FontResource and FontLoader to support embedded resource fonts.
* FileFontFamily: Represents a font loaded from the local file system.
  * GetTypeface: Creates and caches an SKTypeface for the font file.
* Fonts: Extension methods for working with fonts.
  * ScalePoints: Scales font points to pixels at the current display density 
  * DrawText: Wraps Canvas.DrawText by scaling the SKFont.Size using ScalePoints
  * Measure: wraps SKFont.MeasureText by scaling the SKFont.Size using ScalePoints 
* TextUtilities: Used by Grid to measure column widths.
* FontFamilyGroupCollection: A collection of FontFamilyGroup.
  * Manages the collection of all FontFamily instances
  * Groups the font families by the first letter of the family name.
  * Synchronizes the FileFonts and Bookmarks settings with the collection. 
* FontFamilyGroup: A collection of font families in a given group.
* FontMetricsProperties: A A NamedValue collection for displaying displaying font metrics

## Text\Unicode
* Range: A Unicode range
* Ranges: the set of Unicode ranges in 0x0000-0xFFFF
* Extended: The set of Unicode ranges from 0x10000 through 0x100000
  * NOTE: This class is not currently used. 
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

## Converters
* JsonConverter - an abstract base class for a JsonConverter
  * Provides basic argument validation
  * Declares abstract OnRead and OnWrite methods.
* JsonExtensions - provides various JSON extension methods
  * Utf8JsonReader extensions for reading property names, verifing JsonTokenType and reporting unexpected tokens and values.
  * JsonSerializationOptions.Add(params JsonConverter[])
* UserSettingsJsonConverter - The JSON implementation for serializing UserSettings.
* FontFamiliesJsonConverter - The JSON implementation for searilizing FontFamiliesSettings.

## ObjectModel
* ObservableObject: An implementation of INotifyPropertyChanged with SetProperty overloads.
  * SetProperty takes an instance of a PropertyChangedEventArgs to allow derived class to statically define 
    PropertyChangedEventArgs instead of creating instances on each call. 
* ObservableProperty - provides a base class for Settings properties.
  * Provides PropertyChanged notification through the parent ObservableObject.
  * Provides directly PropertyChanged notification for the encapsulated value.
  * Allows SettingsPage to present Settings properties as a bindable collection. 
* Command: An implementation of ICommand with IsEnabled for controlling CanExecute.
* OrderedList\<T\>: An ordered list of objects supporting INotifyCollectionChanged and INotifyPropertyChanged.
  * Derives from ReadOnlyCollection\<T\>. 
  * This is a temporary workaround for the CollectionView issue in Maui 9.0.50.
* ReadOnlyOrderedList: A read-only encapsulation of OrderedList\<T\>.
  * Provides the base class for FontFamilyGroupCollection, FontFamilyGroup and FontFamiliesSettings.
* NamedValue\<T\>
  * Provides a strongly typed Name/Value pair 
  * The base class for SettingProperty\<T\> and ObservableProperty\<T\>
  
## Other
* ExtendedUnicode.txt: A text file containing links to the extended Unicode ranges on www.unicode.org
* Images/GlyphView.png: A screen shot of the GlyphView

## Dependencies
* SkiaSharp.Views.Maui.Controls: 3.116.1
* Microsoft.Maui.Controls: 9.0.50
