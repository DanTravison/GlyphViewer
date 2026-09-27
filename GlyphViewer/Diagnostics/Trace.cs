namespace GlyphViewer.Diagnostics;

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Provides methods for writing trace output.
/// </summary>
public static class Trace
{
    #region Fields

    /// <summary>
    /// The currently enabled trace flags.
    /// </summary>
    static TraceFlag _flags = TraceFlag.Default;

    #endregion Fields

    #region TraceFlags

    /// <summary>
    /// Gets or sets the enabled <see cref="TraceFlags"/>.
    /// </summary>
    public static TraceFlag TraceFlags
    {
        get => _flags;
        set
        {
            _flags = value | TraceFlag.Default;
        }
    }

    /// <summary>
    /// Gets the value indicating if the specified <paramref name="flags"/> are enabled.
    /// </summary>
    /// <param name="flags">The <see cref="TraceFlag"/> values to check.</param>
    /// <returns>true if tracing is enabled for the specified <paramref name="flags"/> values; otherwise, false.</returns>
    public static bool IsEnabled(TraceFlag flags)
    {
        return
        (
            flags.HasFlag(TraceFlag.Error)
            ||
            (
                (flags & TraceFlags) != 0
                &&
                TraceFlags.HasFlag(TraceFlag.Diagnostic) == flags.HasFlag(TraceFlag.Diagnostic)
                &&
                (
                    Trace.TraceFlags.HasFlag(TraceFlag.Warning) == flags.HasFlag(TraceFlag.Warning)
                    ||
                    TraceFlags.HasFlag(TraceFlag.Info) == flags.HasFlag(TraceFlag.Info)
                    ||
                    TraceFlags.HasFlag(TraceFlag.Verbose) == flags.HasFlag(TraceFlag.Verbose)
                )
            )
        );
    }

    #endregion TraceFlags

    #region Error/Warning

    /// <summary>
    /// Writes a formatted string message to trace output for a given Type.Member
    /// </summary>
    /// <param name="traceFlag">The <see cref="TraceFlag"/> identifying the trace output.</param>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="memberName">The name of the member or action producing the trace output.</param>
    /// <param name="format">A composite format string.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx for additional details.
    /// </para>
    /// </param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Error(TraceFlag traceFlag, object source, string memberName, string format, params object[] args)
    {
        if (IsEnabled(traceFlag))
        {
            WriteLine(traceFlag | TraceFlag.Error, source, memberName, format, args);
        }
    }

    /// <summary>
    /// Writes a formatted string message to trace output for a given Type.Member
    /// </summary>
    /// <param name="traceFlag">The <see cref="TraceFlag"/> identifying the trace output.</param>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="memberName">The name of the member or action producing the trace output.</param>
    /// <param name="format">A composite format string.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx for additional details.
    /// </para>
    /// </param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Warning(TraceFlag traceFlag, object source, string memberName, string format, params object[] args)
    {
        if (IsEnabled(traceFlag))
        {
            WriteLine(traceFlag | TraceFlag.Warning, source, memberName, format, args);
        }
    }

    #endregion Error/Warning

    #region Line/Value

    /// <summary>
    /// Writes a formatted string message to trace output for a given Type.Member
    /// </summary>
    /// <param name="traceFlag">The <see cref="TraceFlag"/> identifying the trace output.</param>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="memberName">The name of the member or action producing the trace output.</param>
    /// <param name="format">A composite format string.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx for additional details.
    /// </para>
    /// </param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Line(TraceFlag traceFlag, object source, string memberName, string format, params object[] args)
    {
        if (IsEnabled(traceFlag))
        {
            WriteLine(traceFlag, source, memberName, format, args);
        }
    }

    /// <summary>
    /// Writes a formatted string message to trace output for a given Type.Member
    /// </summary>
    /// <param name="traceFlag">The <see cref="TraceFlag"/> identifying the trace output.</param>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="memberName">The name of the member or action producing the trace output.</param>
    public static void Line(TraceFlag traceFlag, object source, string memberName)
    {
        if (IsEnabled(traceFlag))
        {
            WriteLine(traceFlag, source, memberName, null);
        }
    }

    /// <summary>
    /// The value used for a null source.
    /// </summary>
    public const string NullSource = "[NULL]";

    /// <summary>
    /// Writes a value to trace output.
    /// </summary>
    /// <param name="traceFlag">The <see cref="TraceFlag"/> identifying the trace output.</param>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="memberName">The name of the member or action producing the trace output.</param>
    /// <param name="value">The value to trace.</param>
    /// <remarks>
    /// A source string will be written using the format (<paramref name="source"/>.GetType().Name).<paramref name="memberName"/>.
    /// </remarks>
    public static void Value(TraceFlag traceFlag, object source, string memberName, object value = null)
    {
        if (IsEnabled(traceFlag))
        {
            if (value is null)
            {
                WriteLine(traceFlag, source, memberName, null);
            }
            else
            {
                WriteLine(traceFlag, source, memberName, "{0}", value);
            }
        }
    }

    /// <summary>
    /// Wraps the call to the underlying writer.
    /// </summary>
    /// <param name="traceFlag">The <see cref="TraceFlag"/> identifying the trace output.</param>
    /// <param name="memberName">The name of the member or action producing the trace output.</param>
    /// <param name="format">A composite format string.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx for additional details.
    /// </para>
    /// </param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    static void WriteLine(TraceFlag traceFlag, object source, string memberName, string format, params object[] args)
    {
        string traceSource = Source(source, memberName, traceFlag);
        Output(traceSource, format, args);
    }

    #endregion Line/Value

    #region Exceptions

    /// <summary>
    /// Traces an <see cref="System.Exception"/> with a message.
    /// </summary>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="ex">The <see cref="System.Exception"/> to trace.</param>
    /// <param name="format">A composite format string.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx for additional details.
    /// </para>
    /// </param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Exception(object source, Exception ex, string format = null, params object[] args)
    {
        Exception(source, string.Empty, ex, format, args);
    }

    /// <summary>
    /// Traces an <see cref="System.Exception"/> with a message.
    /// </summary>
    /// <param name="source">The source producing the trace output.
    /// <para>
    /// A source can be an object instance, a <see cref="Type"/> or a string name.
    /// </para>
    /// </param>
    /// <param name="memberName">The name of the member producing the output.</param>
    /// <param name="ex">The <see cref="System.Exception"/> to trace.</param>
    /// <param name="format">A composite format string.
    /// <para>
    /// See https://msdn.microsoft.com/en-us/library/txafckwd(v=vs.110).aspx for additional details.
    /// </para>
    /// </param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    public static void Exception(object source, string memberName, Exception ex, string format = null, params object[] args)
    {
        string traceSource = Source(source, memberName, TraceFlag.Error);

        if (!string.IsNullOrEmpty(format))
        {
            Output(traceSource, format, args);
        }
        Output(traceSource, ex.Message);
        Output(traceSource, ex.StackTrace);

        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
            Output(traceSource, ex.Message);
            Output(traceSource, ex.StackTrace);
        }
    }

    /// <summary>
    /// Gets a string representation of a <see cref="System.Exception"/>.
    /// </summary>
    /// <param name="ex">The <see cref="System.Exception"/> to format.</param>
    /// <returns>A string representing the <paramref name="ex"/> and the nested inner exceptions.</returns>
    public static string FormatException(Exception ex)
    {
        StringBuilder sb = new();
        sb.AppendLine(ex.Message);
        sb.AppendLine(ex.StackTrace);
        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
            sb.AppendLine();
            sb.AppendLine(ex.Message);
            sb.AppendLine(ex.StackTrace);
        }
        return sb.ToString();
    }

    #endregion Exceptions

    #region Utilities

    /// <summary>
    /// Format the trace source string
    /// </summary>
    /// <param name="source">The source of the trace.</param>
    /// <param name="memberName">The member that produced the trace.</param>
    /// <param name="flag">The <see cref="TraceFlag"/> to control output.</param>
    /// <returns>A formatted string identifying the trace source.</returns>
    static string Source(object source, string memberName, TraceFlag flag)
    {
        string sourceName;

        if (source is not null)
        {
            if (source is Type type)
            {
                sourceName = type.Name;
            }
            else
            {
                type = source.GetType();
                if (type == typeof(string))
                {
                    sourceName = (string)source;
                }
                else
                {
                    sourceName = type.Name;
                }
            }
        }
        else
        {
            sourceName = NullSource;
        }

        string traceFlag;
        if (flag != TraceFlag.Error && flag.HasFlag(TraceFlag.Error))
        {
            flag &= ~TraceFlag.Error;
            traceFlag = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", flag, TraceFlag.Error);
        }
        else if (flag != TraceFlag.Warning && flag.HasFlag(TraceFlag.Warning))
        {
            flag &= ~TraceFlag.Warning;
            traceFlag = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", flag, TraceFlag.Warning);
        }
        else
        {
            traceFlag = flag.ToString();
        }
        string format = string.IsNullOrEmpty(memberName) ? "{0}.{1}" : "{0}.{1}.{2}";
        return string.Format(format, sourceName, traceFlag, memberName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Output(string traceSource, string format, params object[] args)
    {
        string message;
        if (args is not null && args.Length > 0)
        {
            message = string.Format(CultureInfo.InvariantCulture, format, args);
        }
        else
        {
            message = format;
        }
        System.Diagnostics.Trace.WriteLine(message, traceSource);
    }

    #endregion Utilities
}
