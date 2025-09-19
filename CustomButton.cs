using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;  // for attributes like Browsable

public class CustomButton : Button
{
    [Browsable(true)]
    [Category("Appearance")]
    [Description("The color of the button border.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColor { get; set; } = Color.Black;

    [Browsable(true)]
    [Category("Appearance")]
    [Description("The color of the button border when selected.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColorSelected { get; set; } = Color.Blue;

    [Browsable(true)]
    [Category("Appearance")]
    [Description("The thickness of the button border.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int BorderThickness { get; set; } = 1;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColorHovered { get; set; } = Color.Blue;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColorPressed { get; set; } = Color.DarkBlue;

    [Browsable(true)]
    [Category("Appearance")]
    [Description("The color of the button border when disabled.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BorderColorDisabled { get; set; } = Color.DimGray;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BackgroundColor { get; set; } = Color.White;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BackgroundColorHovered { get; set; } = Color.LightBlue;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BackgroundColorPressed { get; set; } = Color.LightSkyBlue;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BackgroundColorDisabled { get; set; } = Color.LightGray;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color BackgroundColorSelected { get; set; } = Color.LightGreen;

    private bool _isHovered = false;
    private bool _isPressed = false;
    private bool _isSelected = false;

    [Category("Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Browsable(false)]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value != _isSelected)
            {
                _isSelected = value;
                Invalidate();
            }
        }
    }

    public CustomButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        BackColor = Color.Transparent;
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.UserPaint |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        _isPressed = true;
        Invalidate();
        base.OnMouseDown(mevent);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        _isPressed = false;
        Invalidate();
        base.OnMouseUp(mevent);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        Invalidate();
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        Invalidate();
        base.OnLostFocus(e);
    }

    // Painting
    protected override void OnPaint(PaintEventArgs pevent)
    {
        base.OnPaint(pevent);

        // Choose colors based on state
        Color bgColor = BackgroundColor;
        Color borderColor = BorderColor;

        if (!Enabled)
        {
            bgColor = BackgroundColorDisabled;
            borderColor = BorderColorDisabled;
        }
        else if (_isPressed)
        {
            bgColor = BackgroundColorPressed;
            borderColor = BorderColorPressed;
        }
        else if (_isHovered)
        {
            bgColor = BackgroundColorHovered;
            borderColor = BorderColorHovered;
        }
        else if (_isSelected)
        {
            bgColor = BackgroundColorSelected;
            borderColor = BorderColorSelected;
        }

        // Fill background
        using (SolidBrush brush = new SolidBrush(bgColor))
            pevent.Graphics.FillRectangle(brush, ClientRectangle);

        // Draw text
        TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle,
            ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

        // Draw border
        using (Pen pen = new Pen(borderColor, BorderThickness * (Focused ? 2 : 1)))
        {
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            pevent.Graphics.DrawRectangle(pen, rect);
        }
    }
}
