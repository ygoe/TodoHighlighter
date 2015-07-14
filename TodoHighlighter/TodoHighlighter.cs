using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace TodoHighlighter
{
	/// <summary>
	/// TodoHighlighter places red boxes behind all the "TODO"s in the editor window
	/// </summary>
	public class TodoHighlighter
	{
		private IAdornmentLayer adornmentLayer;
		private IWpfTextView textView;
		private Dictionary<string, FormatInfo> keywordFormats = new Dictionary<string, FormatInfo>();

		public TodoHighlighter(IWpfTextView view)
		{
			textView = view;
			adornmentLayer = view.GetAdornmentLayer("TodoHighlighter");

			// Listen to any event that changes the layout (text changes, scrolling, etc)
			textView.LayoutChanged += OnLayoutChanged;

			// Create the brushes and pens to color the box behind the keywords
			keywordFormats.Add(
				"TODO",
				new FormatInfo(
					Color.FromArgb(0x40, 0xff, 0x10, 0x00),
					Colors.Transparent));
			keywordFormats.Add(
				"DEBUG",
				new FormatInfo(
					Color.FromArgb(0x80, 0xff, 0xd0, 0x00),
					Colors.Transparent));
		}

		/// <summary>
		/// On layout change add the adornment to any reformatted lines
		/// </summary>
		private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs args)
		{
			foreach (ITextViewLine line in args.NewOrReformattedLines)
			{
				this.CreateVisuals(line);
			}
		}

		/// <summary>
		/// Within the given line add the scarlet box behind the a
		/// </summary>
		private void CreateVisuals(ITextViewLine line)
		{
			// Grab a reference to the lines in the current TextView
			IWpfTextViewLineCollection textViewLines = textView.TextViewLines;
			int start = line.Start;
			int end = line.End;
			List<Geometry> geometries = new List<Geometry>();

			var keywordFirstLetters = keywordFormats.Keys
				.Select(k => k[0])
				.Distinct()
				.ToArray();

			// Loop through each character and place a box around any registered keyword
			for (int i = start; i < end; i++)
			{
				if (keywordFirstLetters.Contains(textView.TextSnapshot[i]))   // Performance optimisation
				{
					foreach (var kvp in keywordFormats)
					{
						string keyword = kvp.Key;

						if (textView.TextSnapshot[i] == keyword[0] &&   // Performance optimisation
							i <= end - keyword.Length &&
							textView.TextSnapshot.GetText(i, keyword.Length) == keyword)
						{
							SnapshotSpan span = new SnapshotSpan(textView.TextSnapshot, Span.FromBounds(i, i + keyword.Length));
							Geometry markerGeometry = textViewLines.GetMarkerGeometry(span);
							if (markerGeometry != null)
							{
								if (!geometries.Any(g => g.FillContainsWithDetail(markerGeometry) > IntersectionDetail.Empty))
								{
									geometries.Add(markerGeometry);
									AddMarker(span, markerGeometry, kvp.Value);
								}
							}
						}
					}
				}
			}
		}

		private void AddMarker(SnapshotSpan span, Geometry markerGeometry, FormatInfo formatInfo)
		{
			GeometryDrawing drawing = new GeometryDrawing(formatInfo.Background, formatInfo.Outline, markerGeometry);
			drawing.Freeze();

			DrawingImage drawingImage = new DrawingImage(drawing);
			drawingImage.Freeze();

			Image image = new Image();
			image.Source = drawingImage;

			// Align the image with the top of the bounds of the text geometry
			Canvas.SetLeft(image, markerGeometry.Bounds.Left);
			Canvas.SetTop(image, markerGeometry.Bounds.Top);

			adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
		}

		/// <summary>
		/// Contains data for a keyword formatting.
		/// </summary>
		private class FormatInfo
		{
			public FormatInfo(Color backgroundColor, Color outlineColor)
			{
				if (backgroundColor.A > 0)
				{
					Background = new SolidColorBrush(backgroundColor);
					Background.Freeze();
				}
				if (outlineColor.A > 0)
				{
					var penBrush = new SolidColorBrush(outlineColor);
					penBrush.Freeze();
					Outline = new Pen(penBrush, 1);
					Outline.Freeze();
				}
			}

			public Brush Background { get; set; }
			public Pen Outline { get; set; }
		}
	}
}
