using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyProduct("TodoHighlighter")]
[assembly: AssemblyTitle("TodoHighlighter")]
[assembly: AssemblyDescription("TODO Highlighter Visual Studio Extension")]
[assembly: AssemblyCopyright("© Yves Goergen")]
[assembly: AssemblyCompany("unclassified software development")]

// Assembly identity version. Must be a dotted-numeric version.
// NOTE: Do not edit. This value is automatically set during the build process.
[assembly: AssemblyVersion("0.0")]

// Repeat for Win32 file version resource because the assembly version is expanded to 4 parts.
// NOTE: Do not edit. This value is automatically set during the build process.
[assembly: AssemblyFileVersion("0.0")]

// Informational version string, used for the About dialog, error reports and the setup script.
// Can be any freely formatted string containing punctuation, letters and revision codes.
[assembly: AssemblyInformationalVersion("0.3.0.{revnum}_{chash:6}{!:+}")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: ComVisible(false)]
