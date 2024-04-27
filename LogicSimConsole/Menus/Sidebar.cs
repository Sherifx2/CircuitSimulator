using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

public class Sidebar
{
    private Dictionary<string, Rectangle> gateButtons;
    private List<Gate> gates = new List<Gate>(); 
    private Font buttonFont;
    private string selectedGateType;
    private int buttonHeight = 50;
    private int buttonWidth;
    private int sidebarWidth;
    private int sidebarHeight;
    private int buttonPadding = 5;

    public Sidebar(int initialWidth, int initialHeight)
    {
        SidebarWidth = initialWidth / 5;
        SidebarHeight = initialHeight;

        gateButtons = new Dictionary<string, Rectangle>();
        buttonFont = new Font("Arial", 12);
        AddGateType("AND Gate");
        AddGateType("OR Gate");
        AddGateType("NOT Gate");
        AddGateType("SWITCH");
    }

    public int SidebarWidth
    {
        get => sidebarWidth;
        set
        {
            sidebarWidth = value;
            buttonWidth = value;
        }
    }

    public int SidebarHeight
    {
        get => sidebarHeight;
        set => sidebarHeight = value;
    }
    private void AddGateType(string gateType)
    {
        int buttonY = gateButtons.Count * buttonHeight + buttonPadding;
        Rectangle buttonArea = new Rectangle(buttonPadding, buttonY, buttonWidth - 2 * buttonPadding, buttonHeight - 2 * buttonPadding);
        gateButtons[gateType] = buttonArea;

    }
    public void UpdateSize(int newWidth, int newHeight)
    {
        SidebarWidth = newWidth / 5;
        SidebarHeight = newHeight;

        int buttonY = 0;
        foreach (var key in gateButtons.Keys)
        {
            gateButtons[key] = new Rectangle(buttonPadding, buttonY + buttonPadding, buttonWidth - 2 * buttonPadding, buttonHeight - 2 * buttonPadding);
            buttonY += buttonHeight + buttonPadding; 
        }
    }

    public void Draw(Graphics graphics)
    {
        SolidBrush brush1 = new SolidBrush(ColorTranslator.FromHtml("#333"));
        SolidBrush brush2 = new SolidBrush(ColorTranslator.FromHtml("#222"));
        SolidBrush brush3 = new SolidBrush(ColorTranslator.FromHtml("#111"));
        SolidBrush brush4 = new SolidBrush(ColorTranslator.FromHtml("#fff"));

        graphics.FillRectangle(brush3, new Rectangle(0, 0, SidebarWidth, SidebarHeight));

        foreach (var button in gateButtons)
        {
            Rectangle buttonArea = button.Value;

            DrawRoundedButton(graphics, buttonArea, 10, brush3, new Pen(brush4));

            graphics.DrawString(button.Key, buttonFont, brush4, buttonArea, new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });

        }
    }

    private void DrawRoundedButton(Graphics graphics, Rectangle bounds, int cornerRadius, Brush fillBrush, Pen borderPen)
    {
        using (GraphicsPath path = new GraphicsPath())
        {
            path.AddArc(bounds.X, bounds.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(bounds.X + bounds.Width - cornerRadius, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(bounds.X, bounds.Y + bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();

            graphics.FillPath(fillBrush, path);

            graphics.DrawPath(borderPen, path);
        }
    }

    public bool IsMouseOverButton(Point mousePos, out string gateType)
    {
        foreach (var button in gateButtons)
        {
            if (button.Value.Contains(mousePos))
            {
                gateType = button.Key;
                selectedGateType = gateType;
                return true;
            }
        }
        gateType = null;
        return false;
    }
    public string GetCurrentlyDraggedGate()
    {
        return selectedGateType;
    }
}
