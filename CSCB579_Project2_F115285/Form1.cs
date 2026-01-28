using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CSCB579_Project2_F115285
{
    // Form1: основният прозорец на проекта.
    // Тук са: рисуването върху bitmap, отваряне/запис на снимка, смяна на език (RESX),
    // и отделен слой с текстове (за да могат да се избират и редактират).
    public partial class Form1 : Form
    {
        // Това е "истинската" картинка: снимка + всички нарисувани линии/дъги/free draw.
        private Bitmap _baseBmp;

        // Временна картинка за preview при Line/Arc, докато влачим мишката.
        private Bitmap _previewBmp;

        // Дали в момента сме в режим рисуване (държим ляв бутон).
        private bool _isDrawing;

        // Начална точка за Line/Arc (взима се при MouseDown).
        private Point _start;

        // Последната точка при Free draw (рисуваме сегменти между точки).
        private Point _prevFree;

        // Текстов слой (не го вграждаме в bitmap-а, за да може да се редактира)

        // Един текст, който седи върху картинката и може да се избира/редактира.
        private sealed class TextItem
        {
            public string Text;
            public Font Font;
            public Color Color;
            public Point Location;     // къде започва текстът (горе вляво)
            public Rectangle Bounds;   // смятаме го при рисуване, за selection с мишка

            public TextItem(string text, Font font, Color color, Point location)
            {
                Text = text;
                Font = font;
                Color = color;
                Location = location;
                Bounds = Rectangle.Empty;
            }
        }

        // Всички текстове, които сме добавили.
        private readonly List<TextItem> _texts = new List<TextItem>();

        // Кой текст е избран (-1 означава “няма”).
        private int _selectedTextIndex = -1;

        // True, когато сме натиснали Add Text и чакаме клик за позиция.
        private bool _isPlacingText = false;

        // Текстът, който ще се поставя (още не е добавен в списъка).
        private TextItem _pendingText;

        // Позиция на “призрачния” текст докато местим мишката.
        private Point _pendingLocation;

        // Добавяме меню Text.
        private ToolStripMenuItem _textRootMenu;
        private ToolStripMenuItem _addTextMenu;
        private ToolStripMenuItem _editTextMenu;
        private ToolStripMenuItem _deleteTextMenu;

        // Конструкторът само вика Designer-а (InitializeComponent).
        public Form1()
        {
            InitializeComponent();
        }

        // При старт: правим платното, закачаме Paint за текста, правим меню “Text”, и чистим при затваряне.
        private void Form1_Load(object sender, EventArgs e)
        {
            EnsureCanvas();
            pictureBoxCanvas.Visible = true;

            // Тук рисуваме текста отгоре (overlay), без да пипаме bitmap-а.
            pictureBoxCanvas.Paint += pictureBoxCanvas_Paint;

            // Добавяме меню “Text” от код.
            EnsureTextMenu();

            // На изход освобождаваме ресурси.
            this.FormClosed += Form1_FormClosed;

            // Ако искаш да стартва винаги на определен език:
            // SetLanguage("bg-BG");
            // SetLanguage("en-US");
        }

        // При затваряне: освобождаваме bitmap-и и font-ове (важно е, за да няма течове).
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _previewBmp?.Dispose();
            _previewBmp = null;

            _baseBmp?.Dispose();
            _baseBmp = null;

            foreach (var t in _texts)
                t.Font?.Dispose();

            _pendingText?.Font?.Dispose();
            _pendingText = null;
        }

        // Проверка: дали UI културата е на български.
        private bool IsBg() => Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "bg";

        // Смяна на езика по “лекционния” начин: CultureInfo + ApplyResources.
        // Пазим bitmap-а, защото ApplyResources може да “ресетне” layout-а/контролите.
        private void SetLanguage(string cultureName)
        {
            var backup = _baseBmp != null ? (Bitmap)_baseBmp.Clone() : null;

            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ApplyResources(this, culture);

            pictureBoxCanvas.Visible = true;
            RestoreCanvasAfterLanguageSwitch(backup);

            bulgarianToolStripMenuItem.Checked = culture.TwoLetterISOLanguageName == "bg";
            englishToolStripMenuItem.Checked = culture.TwoLetterISOLanguageName == "en";

            // Меню “Text” е от код, затова го превеждаме отделно.
            UpdateTextMenuCaptions();
            pictureBoxCanvas.Invalidate();
        }

        // Прилага resx ресурсите към формата + всички контроли/менюта.
        private void ApplyResources(Form form, CultureInfo culture)
        {
            var res = new ComponentResourceManager(form.GetType());

            form.SuspendLayout();

            res.ApplyResources(form, "$this", culture);
            ApplyResourcesToControlTree(res, form, culture);

            form.ResumeLayout(true);
            form.PerformLayout();
        }

        // Рекурсивно обхождаме контролите и им прилагаме превод по Name.
        private void ApplyResourcesToControlTree(ComponentResourceManager res, Control parent, CultureInfo culture)
        {
            foreach (Control c in parent.Controls)
            {
                res.ApplyResources(c, c.Name, culture);

                if (c is MenuStrip ms)
                    ApplyResourcesToToolStripItems(res, ms.Items, culture);

                if (c.HasChildren)
                    ApplyResourcesToControlTree(res, c, culture);
            }
        }

        // Менютата са ToolStripItem-и, затова ги обхождаме отделно (и вложените подменюта също).
        private void ApplyResourcesToToolStripItems(ComponentResourceManager res, ToolStripItemCollection items, CultureInfo culture)
        {
            foreach (ToolStripItem item in items)
            {
                res.ApplyResources(item, item.Name, culture);

                if (item is ToolStripDropDownItem dd && dd.DropDownItems.Count > 0)
                    ApplyResourcesToToolStripItems(res, dd.DropDownItems, culture);
            }
        }

        // Грижи се да имаме bitmap със същия размер като PictureBox-а.
        private void EnsureCanvas()
        {
            if (pictureBoxCanvas.Width <= 0 || pictureBoxCanvas.Height <= 0)
                return;

            if (_baseBmp != null &&
                _baseBmp.Width == pictureBoxCanvas.Width &&
                _baseBmp.Height == pictureBoxCanvas.Height)
                return;

            _baseBmp?.Dispose();
            _baseBmp = new Bitmap(pictureBoxCanvas.Width, pictureBoxCanvas.Height);

            using (var g = Graphics.FromImage(_baseBmp))
                g.Clear(Color.White);

            pictureBoxCanvas.Image = _baseBmp;
        }

        // След смяна на език: връщаме картината (ако размерът е сменен, скалираме).
        private void RestoreCanvasAfterLanguageSwitch(Bitmap backup)
        {
            try
            {
                EnsureCanvas();
                if (_baseBmp == null) return;

                _previewBmp?.Dispose();
                _previewBmp = null;

                if (backup == null)
                {
                    pictureBoxCanvas.Image = _baseBmp;
                    return;
                }

                if (_baseBmp.Width == backup.Width && _baseBmp.Height == backup.Height)
                {
                    _baseBmp.Dispose();
                    _baseBmp = (Bitmap)backup.Clone();
                }
                else
                {
                    using (var g = Graphics.FromImage(_baseBmp))
                    {
                        g.Clear(Color.White);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(backup, new Rectangle(0, 0, _baseBmp.Width, _baseBmp.Height));
                    }
                }

                pictureBoxCanvas.Image = _baseBmp;
            }
            finally
            {
                backup?.Dispose();
            }
        }

        // Прави Pen според чековете (Red/Blue -> Purple, иначе Black).
        private Pen MakePen()
        {
            bool red = cbRed.Checked;
            bool blue = cbBlue.Checked;

            Color c = Color.Black;
            if (red && blue) c = Color.Purple;
            else if (red) c = Color.Red;
            else if (blue) c = Color.Blue;

            return new Pen(c, 2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
        }

        // Помощна функция за Arc: прави правоъгълник от 2 точки.
        private Rectangle MakeRect(Point a, Point b)
        {
            int x = Math.Min(a.X, b.X);
            int y = Math.Min(a.Y, b.Y);
            int w = Math.Abs(a.X - b.X);
            int h = Math.Abs(a.Y - b.Y);

            if (w == 0) w = 1;
            if (h == 0) h = 1;

            return new Rectangle(x, y, w, h);
        }

        // Рисува Line или Arc върху дадения Graphics (ползваме го и за preview, и за commit).
        private void DrawShape(Graphics g, Point a, Point b)
        {
            using (var pen = MakePen())
            {
                if (rbLine.Checked)
                {
                    g.DrawLine(pen, a, b);
                }
                else if (rbArc.Checked)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawArc(pen, MakeRect(a, b), 0, 180);
                }
            }
        }

        // Paint на PictureBox: тук рисуваме текстовете отгоре (и рамка на избрания).
        private void pictureBoxCanvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < _texts.Count; i++)
                DrawTextItem(e.Graphics, _texts[i], i == _selectedTextIndex);

            // Preview на текста, преди да е поставен.
            if (_isPlacingText && _pendingText != null)
            {
                using (var brush = new SolidBrush(Color.FromArgb(160, _pendingText.Color)))
                {
                    var size = TextRenderer.MeasureText(_pendingText.Text, _pendingText.Font);
                    var rect = new Rectangle(_pendingLocation, size);

                    e.Graphics.DrawString(_pendingText.Text, _pendingText.Font, brush, _pendingLocation);

                    using (var pen = new Pen(Color.FromArgb(120, Color.Gray)) { DashStyle = DashStyle.Dash })
                        e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        // Рисува един TextItem и обновява Bounds (за да можем да го “хванем” с клик).
        private void DrawTextItem(Graphics g, TextItem item, bool isSelected)
        {
            if (item == null || string.IsNullOrEmpty(item.Text) || item.Font == null)
                return;

            var size = TextRenderer.MeasureText(item.Text, item.Font);
            item.Bounds = new Rectangle(item.Location, size);

            using (var brush = new SolidBrush(item.Color))
                g.DrawString(item.Text, item.Font, brush, item.Location);

            if (isSelected)
            {
                using (var pen = new Pen(Color.OrangeRed, 2f))
                    g.DrawRectangle(pen, item.Bounds);
            }
        }

        // Опитва да избере текст на дадена точка (обхождаме отгоре-надолу: последният е най-отгоре).
        private bool TrySelectTextAt(Point p)
        {
            for (int i = _texts.Count - 1; i >= 0; i--)
            {
                if (_texts[i].Bounds.Contains(p))
                {
                    _selectedTextIndex = i;
                    pictureBoxCanvas.Invalidate();
                    return true;
                }
            }

            _selectedTextIndex = -1;
            pictureBoxCanvas.Invalidate();
            return false;
        }

        // MouseDown: ако слагаме текст -> поставяме; ако кликнем текст -> селектираме; иначе започваме рисуване.
        private void pictureBoxCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            EnsureCanvas();
            if (_baseBmp == null) return;

            // Поставяне на нов текст.
            if (_isPlacingText && _pendingText != null && e.Button == MouseButtons.Left)
            {
                _pendingText.Location = e.Location;
                _texts.Add(_pendingText);
                _selectedTextIndex = _texts.Count - 1;

                _pendingText = null;
                _isPlacingText = false;

                pictureBoxCanvas.Invalidate();
                return;
            }

            // Ако щракнем върху текст – не рисуваме, само селектираме.
            if (e.Button == MouseButtons.Left && TrySelectTextAt(e.Location))
                return;

            if (e.Button != MouseButtons.Left) return;

            _isDrawing = true;
            _start = e.Location;
            _prevFree = e.Location;
        }

        // MouseMove: ако местим нов текст -> местим preview; ако рисуваме -> free draw или preview за line/arc.
        private void pictureBoxCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPlacingText && _pendingText != null)
            {
                _pendingLocation = e.Location;
                pictureBoxCanvas.Invalidate();
                return;
            }

            if (!_isDrawing || _baseBmp == null) return;

            if (rbFreeDraw.Checked)
            {
                using (var g = Graphics.FromImage(_baseBmp))
                using (var pen = MakePen())
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLine(pen, _prevFree, e.Location);
                }

                _prevFree = e.Location;
                pictureBoxCanvas.Image = _baseBmp;
                pictureBoxCanvas.Invalidate();
                return;
            }

            _previewBmp?.Dispose();
            _previewBmp = (Bitmap)_baseBmp.Clone();

            using (var gPrev = Graphics.FromImage(_previewBmp))
            {
                gPrev.SmoothingMode = SmoothingMode.AntiAlias;
                DrawShape(gPrev, _start, e.Location);
            }

            pictureBoxCanvas.Image = _previewBmp;
            pictureBoxCanvas.Invalidate();
        }

        // MouseUp: приключваме рисуването; при Line/Arc комитваме върху baseBmp.
        private void pictureBoxCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _baseBmp == null) return;
            if (e.Button != MouseButtons.Left) return;

            _isDrawing = false;

            if (!rbFreeDraw.Checked)
            {
                using (var g = Graphics.FromImage(_baseBmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    DrawShape(g, _start, e.Location);
                }
            }

            pictureBoxCanvas.Image = _baseBmp;
            pictureBoxCanvas.Invalidate();

            _previewBmp?.Dispose();
            _previewBmp = null;
        }

        // Не е нужен, но ако е вързан в Designer – оставяме празен.
        private void pictureBoxCanvas_Click(object sender, EventArgs e) { }

        // Clear: изтрива само bitmap-а (текстовете ги оставя, защото са отделен слой).
        private void btnClear_Click(object sender, EventArgs e)
        {
            EnsureCanvas();
            if (_baseBmp == null) return;

            using (var g = Graphics.FromImage(_baseBmp))
                g.Clear(Color.White);

            _previewBmp?.Dispose();
            _previewBmp = null;

            pictureBoxCanvas.Image = _baseBmp;
            pictureBoxCanvas.Invalidate();
        }

        // Exit.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Open image: зареждаме снимка и я рисуваме върху целия canvas (със скалиране).
        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureCanvas();

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = IsBg() ? "Отвори изображение" : "Open Image";
                ofd.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                using (var loaded = Image.FromFile(ofd.FileName))
                using (var g = Graphics.FromImage(_baseBmp))
                {
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(loaded, new Rectangle(0, 0, _baseBmp.Width, _baseBmp.Height));
                }

                pictureBoxCanvas.Image = _baseBmp;
                pictureBoxCanvas.Visible = true;
                pictureBoxCanvas.Invalidate();
            }
        }

        // About: тук са трите имена и фак. номер + какво има в проекта.
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bg = IsBg();

            MessageBox.Show(
                bg
                    ? "CSCB579 – Проект 2\nКалоян Димов\nФак. № F115285\n\nПриложение:\n• Менюта\n• Многоезичен интерфейс\n• Зареждане/Запис на изображения\n• Рисуване (Free/Line/Arc) + Preview\n• Текст върху изображението (редакция/шрифт)\n"
                    : "CSCB579 – Project 2\nKaloyan Dimov\nFaculty No. F115285\n\nApp:\n• Menus\n• Multilanguage UI\n• Load/Save images\n• Drawing (Free/Line/Arc) + Preview\n• Text on image (edit/font)\n",
                bg ? "Относно" : "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        // Language -> Bulgarian
        private void bulgarianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("bg-BG");
        }

        // Language -> English
        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("en-US");
        }

        // Ако Designer е вързал друг handler, прехвърляме към основния.
        private void englishToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            englishToolStripMenuItem_Click(sender, e);
        }

        // Ако Designer е вързал Click на "File" root – не правим нищо.
        private void fileToolStripMenuItem_Click(object sender, EventArgs e) { }

        // Save: правим композит (base + текстове) и го записваме.
        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bmp = BuildCompositeBitmapForSave();
            if (bmp == null) return;

            using (bmp)
            using (var sfd = new SaveFileDialog())
            {
                bool bg = IsBg();
                sfd.Title = bg ? "Запази изображение" : "Save Image";
                sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Bitmap (*.bmp)|*.bmp";
                sfd.DefaultExt = "png";
                sfd.AddExtension = true;

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                string ext = Path.GetExtension(sfd.FileName).ToLowerInvariant();
                ImageFormat fmt = ImageFormat.Png;

                if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
                else if (ext == ".bmp") fmt = ImageFormat.Bmp;

                // JPG: без alpha, затова го правим на 24bpp с бял фон.
                if (fmt == ImageFormat.Jpeg)
                {
                    using (var rgb = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb))
                    using (var g = Graphics.FromImage(rgb))
                    {
                        g.Clear(Color.White);
                        g.DrawImage(bmp, 0, 0);
                        rgb.Save(sfd.FileName, fmt);
                    }
                }
                else
                {
                    bmp.Save(sfd.FileName, fmt);
                }
            }
        }

        // Правим bitmap за запис: baseBmp + всички текстове (без selection рамки).
        private Bitmap BuildCompositeBitmapForSave()
        {
            EnsureCanvas();
            if (_baseBmp == null) return null;

            var composite = new Bitmap(_baseBmp.Width, _baseBmp.Height);
            using (var g = Graphics.FromImage(composite))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.DrawImage(_baseBmp, 0, 0);

                foreach (var t in _texts)
                {
                    if (t == null || string.IsNullOrEmpty(t.Text) || t.Font == null) continue;
                    using (var brush = new SolidBrush(t.Color))
                        g.DrawString(t.Text, t.Font, brush, t.Location);
                }
            }
            return composite;
        }

        // Създава меню “Text” (ако го няма) и връзва обработчици.
        private void EnsureTextMenu()
        {
            if (menuStrip1 == null) return;

            foreach (ToolStripItem it in menuStrip1.Items)
            {
                if (it is ToolStripMenuItem mi && mi.Name == "textToolStripMenuItem")
                {
                    _textRootMenu = mi;
                    return;
                }
            }

            _textRootMenu = new ToolStripMenuItem { Name = "textToolStripMenuItem" };

            _addTextMenu = new ToolStripMenuItem { Name = "addTextToolStripMenuItem" };
            _editTextMenu = new ToolStripMenuItem { Name = "editTextToolStripMenuItem" };
            _deleteTextMenu = new ToolStripMenuItem { Name = "deleteTextToolStripMenuItem" };

            _addTextMenu.Click += AddTextMenu_Click;
            _editTextMenu.Click += EditTextMenu_Click;
            _deleteTextMenu.Click += DeleteTextMenu_Click;

            _textRootMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                _addTextMenu, _editTextMenu, _deleteTextMenu
            });

            // Слагаме “Text” преди Help, ако Help го има.
            int insertIndex = menuStrip1.Items.Count;
            for (int i = 0; i < menuStrip1.Items.Count; i++)
            {
                if (menuStrip1.Items[i] == helpToolStripMenuItem)
                {
                    insertIndex = i;
                    break;
                }
            }

            menuStrip1.Items.Insert(insertIndex, _textRootMenu);

            UpdateTextMenuCaptions();
        }

        // Превеждаме “Text” менюто (то не идва от resx).
        private void UpdateTextMenuCaptions()
        {
            if (_textRootMenu == null) return;

            bool bg = IsBg();
            _textRootMenu.Text = bg ? "Текст" : "Text";
            _addTextMenu.Text = bg ? "Добави текст..." : "Add Text...";
            _editTextMenu.Text = bg ? "Редактирай избрания..." : "Edit Selected...";
            _deleteTextMenu.Text = bg ? "Изтрий избрания" : "Delete Selected";
        }

        // Add Text: първо избираме текст/шрифт/цвят, после чакаме клик за позиция.
        private void AddTextMenu_Click(object sender, EventArgs e)
        {
            if (!ShowTextEditorDialog(
                    title: IsBg() ? "Добавяне на текст" : "Add Text",
                    initialText: "",
                    initialFont: new Font("Segoe UI", 18f, FontStyle.Bold),
                    initialColor: Color.Black,
                    out string text,
                    out Font font,
                    out Color color))
            {
                return;
            }

            _pendingText?.Font?.Dispose();
            _pendingText = new TextItem(text, font, color, new Point(10, 10));

            _pendingLocation = new Point(10, 10);
            _isPlacingText = true;

            this.Text = IsBg()
                ? "CSCB579 – Проект 2 (кликни върху картинката, за да сложиш текста)"
                : "CSCB579 – Project 2 (click on the image to place the text)";

            pictureBoxCanvas.Invalidate();
        }

        // Edit Selected: работи само ако имаме избран текст.
        private void EditTextMenu_Click(object sender, EventArgs e)
        {
            if (_selectedTextIndex < 0 || _selectedTextIndex >= _texts.Count)
            {
                MessageBox.Show(
                    IsBg() ? "Първо избери текст с клик върху него." : "Click a text first to select it.",
                    IsBg() ? "Няма избран текст" : "No selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var item = _texts[_selectedTextIndex];

            if (!ShowTextEditorDialog(
                    title: IsBg() ? "Редакция на текст" : "Edit Text",
                    initialText: item.Text,
                    initialFont: item.Font,
                    initialColor: item.Color,
                    out string newText,
                    out Font newFont,
                    out Color newColor))
            {
                return;
            }

            item.Font?.Dispose();
            item.Text = newText;
            item.Font = newFont;
            item.Color = newColor;

            pictureBoxCanvas.Invalidate();
        }

        // Delete Selected: маха избрания текст.
        private void DeleteTextMenu_Click(object sender, EventArgs e)
        {
            if (_selectedTextIndex < 0 || _selectedTextIndex >= _texts.Count)
                return;

            _texts[_selectedTextIndex].Font?.Dispose();
            _texts.RemoveAt(_selectedTextIndex);
            _selectedTextIndex = -1;

            pictureBoxCanvas.Invalidate();
        }

        // Диалог за текст + шрифт + цвят. Връща true само ако има OK и текстът не е празен.
        private bool ShowTextEditorDialog(
            string title,
            string initialText,
            Font initialFont,
            Color initialColor,
            out string text,
            out Font font,
            out Color color)
        {
            text = initialText;
            font = null;
            color = initialColor;

            Font chosenFont = (Font)initialFont.Clone();
            Color chosenColor = initialColor;

            using (var dlg = new Form())
            using (var tb = new TextBox())
            using (var btnFont = new Button())
            using (var btnColor = new Button())
            using (var lblPreview = new Label())
            using (var btnOk = new Button())
            using (var btnCancel = new Button())
            {
                dlg.Text = title;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ClientSize = new Size(520, 220);

                tb.Multiline = true;
                tb.ScrollBars = ScrollBars.Vertical;
                tb.SetBounds(12, 12, 496, 80);
                tb.Text = initialText;

                btnFont.Text = IsBg() ? "Шрифт..." : "Font...";
                btnFont.SetBounds(12, 105, 120, 32);

                btnColor.Text = IsBg() ? "Цвят..." : "Color...";
                btnColor.SetBounds(142, 105, 120, 32);

                lblPreview.Text = IsBg() ? "Преглед" : "Preview";
                lblPreview.SetBounds(12, 145, 496, 28);
                lblPreview.Font = chosenFont;
                lblPreview.ForeColor = chosenColor;

                btnOk.Text = "OK";
                btnOk.SetBounds(308, 180, 95, 30);
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Text = IsBg() ? "Отказ" : "Cancel";
                btnCancel.SetBounds(413, 180, 95, 30);
                btnCancel.DialogResult = DialogResult.Cancel;

                dlg.Controls.AddRange(new Control[] { tb, btnFont, btnColor, lblPreview, btnOk, btnCancel });
                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                void RefreshPreview()
                {
                    lblPreview.Text = string.IsNullOrWhiteSpace(tb.Text)
                        ? (IsBg() ? "Преглед" : "Preview")
                        : tb.Text.Replace("\r\n", " ");
                    lblPreview.Font = chosenFont;
                    lblPreview.ForeColor = chosenColor;
                }

                tb.TextChanged += (s, e) => RefreshPreview();

                btnFont.Click += (s, e) =>
                {
                    using (var fd = new FontDialog())
                    {
                        fd.Font = chosenFont;

                        if (fd.ShowDialog(dlg) == DialogResult.OK)
                        {
                            chosenFont.Dispose();
                            chosenFont = (Font)fd.Font.Clone();
                            RefreshPreview();
                        }
                    }
                };

                btnColor.Click += (s, e) =>
                {
                    using (var cd = new ColorDialog())
                    {
                        cd.Color = chosenColor;
                        if (cd.ShowDialog(dlg) == DialogResult.OK)
                        {
                            chosenColor = cd.Color;
                            RefreshPreview();
                        }
                    }
                };

                RefreshPreview();

                var result = dlg.ShowDialog(this);
                if (result != DialogResult.OK)
                {
                    chosenFont.Dispose();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    MessageBox.Show(
                        IsBg() ? "Моля въведи текст." : "Please enter some text.",
                        IsBg() ? "Липсва текст" : "Missing text",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    chosenFont.Dispose();
                    return false;
                }

                text = tb.Text;
                font = chosenFont; // връщаме шрифта към caller-а (той ще го dispose-не по-късно)
                color = chosenColor;
                return true;
            }
        }
    }
}
