namespace GlyphViewer.ViewModels;

using GlyphViewer.ObjectModel;
using System.ComponentModel;

/// <summary>
/// Provides an abstract base class for a page navigator.
/// </summary>
public abstract class PageNavigator
{
    /// <summary>
    /// Defines the state of the page navigation.
    /// </summary>
    public enum NavigationState
    {
        /// <summary>
        /// The page is closed.
        /// </summary>
        Closed,
        /// <summary>
        /// The page is closing.
        /// </summary>
        Closing,
        /// <summary>
        /// The page is opening
        /// </summary>
        Opening,
        /// <summary>
        /// The page is open.
        /// </summary>
        Open,
    }

    #region Fields

    readonly object _bindingContext;
    ContentPage _page;
    NavigationState _state;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    /// <param name="isModal">true if the page is modal; otherwise, false.</param>
    /// <param name="bindingContext">The optional <see cref="BindableObject.BindingContext"/> for the page.</param>
    protected PageNavigator(bool isModal, object bindingContext = null)
    {
        IsModal = isModal;
        _bindingContext = bindingContext;

        OpenCommand = new Command(OnOpen)
        {
            IsEnabled = true
        };
        CloseCommand = new Command(OnClose)
        {
            IsEnabled = false
        };
    }

    #region Properties

    /// <summary>
    /// Gets the command that opens the page.
    /// </summary>
    public Command OpenCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command that opens the page.
    /// </summary>
    public Command CloseCommand
    {
        get;
    }

    /// <summary>
    /// Gets the value indicating if the page is modal.
    /// </summary>
    public bool IsModal
    {
        get;
    }

    /// <summary>
    /// Gets the <see cref="NavigationState"/> of the page.
    /// </summary>
    public NavigationState State
    {
        get
        {
            return _state;
        }
        private set
        {
            if (_state != value)
            {
                _state = value;
                CloseCommand.IsEnabled = _state == NavigationState.Open;
                OpenCommand.IsEnabled = _state == NavigationState.Closed;
            }
        }
    }

    #endregion Properties

    #region Open/Close

    /// <summary>
    /// Opens the page
    /// </summary>
    void OnOpen()
    {
        if (State == NavigationState.Closed)
        {
            _page = CreatePage(_bindingContext);
            Subscribe();
            
            if (IsModal)
            {
                App.Navigation.PushModalAsync(_page);
            }
            else
            {
                App.Navigation.PushAsync(_page);
            }
        }
    }

    /// <summary>
    /// Closes the current page.
    /// </summary>
    void OnClose()
    {
        if (State == NavigationState.Open)
        {
            State = NavigationState.Closing;
            if (IsModal)
            {
                App.Navigation.PopModalAsync();
            }
            else
            {
                App.Navigation.PopAsync();
            }
        }
    }

    #endregion Open/Close

    #region Event Handlers

    void Subscribe()
    {
        _page.NavigatedTo += OnPageNavigatedTo;
        _page.NavigatingFrom += OnPageNavigatingFrom;
        _page.Unloaded += OnPageUnloaded;
     
        State = NavigationState.Opening;
    }

    void Unsubscribe()
    {
        _page.NavigatedTo -= OnPageNavigatedTo;
        _page.NavigatingFrom -= OnPageNavigatingFrom;
        _page.Unloaded -= OnPageUnloaded;
        // Clear all bindings
        _page.BindingContext = null;
        _page = null;
 
        State = NavigationState.Closed;
    }

    private void OnPageNavigatingFrom(object sender, NavigatingFromEventArgs e)
    {
        if (ReferenceEquals(_page, sender))
        {
            State = NavigationState.Closed;
        }
    }

    private void OnPageNavigatedTo(object sender, NavigatedToEventArgs e)
    {
        if (ReferenceEquals(_page, sender))
        {
            State = NavigationState.Open;
        }
    }

    private void OnPageUnloaded(object sender, EventArgs e)
    {
        if (ReferenceEquals(sender, _page))
        {
            Unsubscribe();
        }
    }

    #endregion Event Handlers

    #region Abstract Methods

    /// <summary>
    /// Creates an instance of the <see cref="ContentPage"/>.
    /// </summary>
    /// <param name="bindingContext">The optional <see cref="INotifyPropertyChanged"/> <see cref="BindableObject.BindingContext"/>.</param>
    /// <returns>A new instance of the <see cref="ContentPage"/>.</returns>
    protected abstract ContentPage CreatePage(object bindingContext);

    #endregion Abstract Methods
}

/// <summary>
/// Provides a strongly typed <see cref="PageNavigator"/>.
/// </summary>
/// <typeparam name="T">The type of <see cref="ContentPage"/>.</typeparam>
public class PageNavigator<T> : PageNavigator
    where T : ContentPage
{
    /// <summary>
    /// Initilizes a new instance of this class.
    /// </summary>
    /// <param name="isModal">true if the page is modal; otherwise, false.</param>
    /// <param name="bindingContext">The optional <see cref="BindableObject.BindingContext"/> for the <see cref="ContentPage"/>.</param>
    public PageNavigator(bool isModal, object bindingContext)
        : base(isModal, bindingContext)
    {
    }

    /// <summary>
    /// Creates a new instance of the <typeparamref name="T"/> <see cref="ContentPage"/>.
    /// </summary>
    /// <param name="bindingContext">The optional <see cref="BindableObject.BindingContext"/> for the <see cref="ContentPage"/>.</param>
    /// <returns>A new instance of a <typeparamref name="T"/> <see cref="ContentPage"/>.</returns>
    protected override ContentPage CreatePage(object bindingContext)
    {
        T page = Activator.CreateInstance<T>();
        if (bindingContext is not null)
        {
            page.BindingContext = bindingContext;
        }
        return page;
    }
}
