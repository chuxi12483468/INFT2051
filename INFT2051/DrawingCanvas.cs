using System.Text.Json;

namespace INFT2051;

public class DrawingStroke
{
    public List<SerializablePoint> Points { get; set; } = new();
    public string ColorHex { get; set; } = "#000000";
    public float StrokeSize { get; set; } = 3;
    public bool IsEraser { get; set; } = false;
}

public class SerializablePoint
{
    public float X { get; set; }
    public float Y { get; set; }
}

public class DrawingCanvas : IDrawable
{
    public List<DrawingStroke> Strokes { get; private set; } = new();

    public Color CurrentColor { get; set; } = Colors.Black;
    public float CurrentStrokeSize { get; set; } = 3;
    public bool IsEraserMode { get; set; } = false;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        foreach (var stroke in Strokes)
        {
            if (stroke.Points.Count < 2)
                continue;

            var color = stroke.IsEraser
                ? Colors.White
                : Color.FromArgb(stroke.ColorHex);

            canvas.StrokeColor = color;
            canvas.StrokeSize = stroke.StrokeSize;

            for (int i = 0; i < stroke.Points.Count - 1; i++)
            {
                canvas.DrawLine(
                    stroke.Points[i].X, stroke.Points[i].Y,
                    stroke.Points[i + 1].X, stroke.Points[i + 1].Y);
            }
        }
    }

    public void StartStroke(float x, float y)
    {
        var stroke = new DrawingStroke
        {
            ColorHex = CurrentColor.ToArgbHex(),
            StrokeSize = CurrentStrokeSize,
            IsEraser = IsEraserMode,
            Points = new List<SerializablePoint>
            {
                new SerializablePoint { X = x, Y = y }
            }
        };

        Strokes.Add(stroke);
    }

    public void AddPoint(float x, float y)
    {
        if (Strokes.Count == 0)
            return;

        Strokes[^1].Points.Add(new SerializablePoint { X = x, Y = y });
    }

    public void Clear()
    {
        Strokes.Clear();
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(Strokes);
    }

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
            Strokes = result ?? new List<DrawingStroke>();
        }
        catch
        {
            Strokes = new List<DrawingStroke>();
        }
    }
}

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