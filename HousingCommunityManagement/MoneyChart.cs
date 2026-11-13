using HousingCommunityManagement.Core;

namespace HousingCommunityManagement;
public sealed class MoneyChart : Control
{
    private decimal[][] values = [new decimal[12], new decimal[12], new decimal[12], new decimal[12]];
    private readonly Color[] colors = [Color.SeaGreen, Color.SteelBlue, Color.DarkGoldenrod, Color.MediumPurple];
    private readonly string[] names = ["Receipts (maintenance & other)", "Payments (bills & other)", "Deposits received", "Deposits returned"];
    private readonly ToolTip tooltip = new();
    private int hoveredMonth = -1;
    public MoneyChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            tooltip.Dispose();
        base.Dispose(disposing);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        var month = e.Y < 65 ? -1 : Math.Clamp((int)((e.X - 6) / ((Width - 12f) / 12)), 0, 11);
        if (month == hoveredMonth)
            return;
        hoveredMonth = month;
        tooltip.SetToolTip(this, month < 0 ? "" : new DateTime(2000, month + 1, 1).ToString("MMMM") + "\n" + string.Join("\n", names.Select((name, i) => $"{name}: RM {values[i][month]:N2}")));
    }

    public void SetValues(params decimal[][] series)
    {
        values = series;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var max = Math.Max(1m, values.SelectMany(v => v).Max(v => Math.Abs(v)));
        for (var s = 0; s < 4; s++)
        {
            using var brush = new SolidBrush(colors[s]);
            e.Graphics.FillRectangle(brush, s % 2 * Width / 2 + 2, s / 2 * 22 + 5, 9, 9);
            TextRenderer.DrawText(e.Graphics, names[s], Font, new Point(s % 2 * Width / 2 + 15, s / 2 * 22 + 1), ForeColor, TextFormatFlags.NoPrefix);
        }

        var negative = values.SelectMany(v => v).Any(v => v < 0);
        var top = 65;
        var baseline = negative ? (Height - 22 + top) / 2 : Height - 32;
        var scale = Math.Max(1, baseline - top);
        var group = (Width - 12f) / 12;
        TextRenderer.DrawText(e.Graphics, $"Scale: RM {max:N0} · hover over a month for amounts", Font, new Point(2, 44), ForeColor, TextFormatFlags.NoPrefix);
        e.Graphics.DrawLine(Pens.LightGray, 2, baseline, Width - 2, baseline);
        for (var m = 0; m < 12; m++)
        {
            for (var s = 0; s < 4; s++)
            {
                var height = (float)(Math.Abs(values[s][m]) / max) * scale;
                using var brush = new SolidBrush(colors[s]);
                e.Graphics.FillRectangle(brush, 6 + m * group + s * group / 5, values[s][m] < 0 ? baseline : baseline - height, Math.Max(1, group / 5 - 1), height);
            }

            TextRenderer.DrawText(e.Graphics, new DateTime(2000, m + 1, 1).ToString("MMM"), Font, new Point((int)(4 + m * group), Height - 22), ForeColor, TextFormatFlags.NoPrefix);
        }
    }
}
