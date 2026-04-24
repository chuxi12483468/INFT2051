using System.Text.Json;

namespace INFT2051;

// Represents a single drawing stroke (a continuous line)
public class DrawingStroke
{
    // List of points that make up the stroke
    public List<SerializablePoint> Points { get; set; } = new();

    // Stroke color stored as hex string (e.g., #FF000000)
    public string ColorHex { get; set; } = "#000000";

    // Thickness of the stroke
    public float StrokeSize { get; set; } = 3;

    // Indicates whether this stroke is an eraser action
    public bool IsEraser { get; set; } = false;
}

// Serializable point structure for storing drawing coordinates
public class SerializablePoint
{
    public float X { get; set; }
    public float Y { get; set; }
}

// Custom drawing canvas implementing MAUI's IDrawable interface
public class DrawingCanvas : IDrawable
{
    // List of all strokes drawn by the user
    public List<DrawingStroke> Strokes { get; private set; } = new();

    // Current drawing settings
    public Color CurrentColor { get; set; } = Colors.Black;
    public float CurrentStrokeSize { get; set; } = 3;
    public bool IsEraserMode { get; set; } = false;

    // Render all strokes onto the canvas
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        foreach (var stroke in Strokes)
        {
            // Ignore strokes with insufficient points
            if (stroke.Points.Count < 2)
                continue;

            // Use white color if eraser mode, otherwise use stored color
            var color = stroke.IsEraser
                ? Colors.White
                : Color.FromArgb(stroke.ColorHex);

            canvas.StrokeColor = color;
            canvas.StrokeSize = stroke.StrokeSize;

            // Draw line segments between consecutive points
            for (int i = 0; i < stroke.Points.Count - 1; i++)
            {
                canvas.DrawLine(
                    stroke.Points[i].X, stroke.Points[i].Y,
                    stroke.Points[i + 1].X, stroke.Points[i + 1].Y);
            }
        }
    }

    // Start a new stroke when user begins drawing
    public void StartStroke(float x, float y)
    {
        var stroke = new DrawingStroke
        {
            ColorHex = CurrentColor.ToArgbHex(), // Convert color to hex format
            StrokeSize = CurrentStrokeSize,
            IsEraser = IsEraserMode,

            // Initialize with first point
            Points = new List<SerializablePoint>
            {
                new SerializablePoint { X = x, Y = y }
            }
        };

        Strokes.Add(stroke);
    }

    // Add a point to the current stroke during drawing
    public void AddPoint(float x, float y)
    {
        if (Strokes.Count == 0)
            return;

        Strokes[^1].Points.Add(new SerializablePoint { X = x, Y = y });
    }

    // Clear all strokes from the canvas
    public void Clear()
    {
        Strokes.Clear();
    }

    // Convert drawing data into JSON string for storage
    public string Serialize()
    {
        return JsonSerializer.Serialize(Strokes);
    }

    // Restore drawing data from JSON string
    public void Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Strokes = new List<DrawingStroke>();
            return;
        }

        try
        {
            var result = JsonSerializer.Deserialize<List<DrawingStroke>>(json);

            // Ensure valid result
            Strokes = result ?? new List<DrawingStroke>();
        }
        catch
        {
            // Reset if deserialization fails
            Strokes = new List<DrawingStroke>();
        }
    }
}

// Extension method to convert MAUI Color to ARGB hex string
public static class ColorExtensions
{
    public static string ToArgbHex(this Color color)
    {
        int a = (int)(color.Alpha * 255);
        int r = (int)(color.Red * 255);
        int g = (int)(color.Green * 255);
        int b = (int)(color.Blue * 255);

        return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
    }
}