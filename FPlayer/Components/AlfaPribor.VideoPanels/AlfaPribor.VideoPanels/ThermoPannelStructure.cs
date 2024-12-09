using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AlfaPribor.VideoPanels
{
   
    //class ThermoPannelStructure
    //{
    //    enum ToolbarImagesNames
    //    {
    //        /// <summary>Мастер канал включен</summary>
    //        MasterChannelOn = 0,
    //        /// <summary>Мастер канал отключен</summary>
    //        MasterChannelOff = 1,
    //        /// <summary>Графические элементы</summary>
    //        Pencil = 2,
    //        /// <summary>Палитра серая</summary>
    //        PaletteGrey = 3,
    //        /// <summary>Палитра цветная</summary>
    //        PaletteColor = 4,
    //        /// <summary>Палитра красно-синяя</summary>
    //        PaletteRed = 5,
    //        /// <summary>Печать кадра</summary>
    //        PrintImage = 6,
    //        /// <summary>Сохранение кадра</summary>
    //        SaveImage = 7
    //    };
    //    /// <summary>Картинки панели</summary>
    //    ImageList ToolBarImages;

    //    string[] ToolbarDrawingItemsNames = { "Границы анализа", 
    //                                          "Границы калибровочного типа",
    //                                          "Уровень горизонта",
    //                                          "Отображать надписи",
    //                                          "Функция распределения температур",
    //                                          "Уровень загрузки",
    //                                          "Уровень загрузки по документам",
    //                                          "Шкала температур",
    //                                          "Сигнал в указанной точке" };

    //    /// <summary>Загрузка картинок</summary>
    //    void LoadImages()
    //    {
    //        ToolBarImages = new ImageList();
    //        ToolBarImages.ColorDepth = ColorDepth.Depth32Bit;
    //        ToolBarImages.ImageSize = new Size(16, 16);
    //        for (int i = 0; i < Enum.GetNames(typeof(ToolbarImagesNames)).Length; i++)
    //        {
    //            if (i == (int)ToolbarImagesNames.MasterChannelOn) ToolBarImages.Images.Add(Properties.Resources.master);
    //            if (i == (int)ToolbarImagesNames.MasterChannelOff) ToolBarImages.Images.Add(Properties.Resources.master_off);
    //            if (i == (int)ToolbarImagesNames.PaletteColor) ToolBarImages.Images.Add(Properties.Resources.rainbow);
    //            if (i == (int)ToolbarImagesNames.PaletteGrey) ToolBarImages.Images.Add(Properties.Resources.grayscale);
    //            if (i == (int)ToolbarImagesNames.PaletteRed) ToolBarImages.Images.Add(Properties.Resources.hot_red);
    //            if (i == (int)ToolbarImagesNames.Pencil) ToolBarImages.Images.Add(Properties.Resources.graph);
    //            if (i == (int)ToolbarImagesNames.PrintImage) ToolBarImages.Images.Add(Properties.Resources.print);
    //            if (i == (int)ToolbarImagesNames.SaveImage) ToolBarImages.Images.Add(Properties.Resources.save);
    //            //ToolBarImages.Images.Add(GetDefaultImage());
    //        }
    //    }

    //    Image GetDefaultImage()
    //    {
    //        return Image.FromHbitmap((new Bitmap(16, 16)).GetHbitmap());
    //    }

    //    /// <summary>Создание кнопки списка палитр</summary>
    //    public ToolStripDropDownButton CreatePaletteBox()
    //    {
    //        //Если тепловизиор не активирован - выход
    //        if (!Settings.Thermovision.Enabled.ToBool()) return;
    //        //Номер панели тепловизора
    //        int ipanel = 1 - Settings.Interface.TelecameraWindow.ToInt();
    //        ToolStripDropDownButton ItemPalette = (ToolStripDropDownButton)ToolStrip[ipanel].Items["ItemPalette"];
    //        if (ItemPalette == null)
    //        {
    //            Image image = ToolBarImages.Images[(int)ToolbarImagesNames.PaletteGrey];
    //            if ((DrawThermoFrames.Palettes)Settings.ThermoDrawingFlags.Palette.ToInt() == DrawThermoFrames.Palettes.Rainbow) image = ToolBarImages.Images[(int)ToolbarImagesNames.PaletteColor];
    //            if ((DrawThermoFrames.Palettes)Settings.ThermoDrawingFlags.Palette.ToInt() == DrawThermoFrames.Palettes.RedHot) image = ToolBarImages.Images[(int)ToolbarImagesNames.PaletteRed];
    //            ItemPalette = new ToolStripDropDownButton(image);
    //            ItemPalette.Name = "ItemPalette";
    //            ItemPalette.Alignment = ToolStripItemAlignment.Right;
    //            ItemPalette.ToolTipText = "Палитра";
    //            ToolStripDropDown drop = new ToolStripDropDown();
    //            ToolStripItem item_grey = drop.Items.Add("Черно-белая", ToolBarImages.Images[(int)ToolbarImagesNames.PaletteGrey]);
    //            item_grey.Tag = (int)DrawThermoFrames.Palettes.Gray;
    //            item_grey.MouseDown += new MouseEventHandler(item_MouseDown);
    //            ToolStripItem item_color = drop.Items.Add("Цветная", ToolBarImages.Images[(int)ToolbarImagesNames.PaletteColor]);
    //            item_color.Tag = (int)DrawThermoFrames.Palettes.Rainbow;
    //            item_color.MouseDown += new MouseEventHandler(item_MouseDown);
    //            ToolStripItem item_redhot = drop.Items.Add("Металл", ToolBarImages.Images[(int)ToolbarImagesNames.PaletteRed]);
    //            item_redhot.Tag = (int)DrawThermoFrames.Palettes.RedHot;
    //            item_redhot.MouseDown += new MouseEventHandler(item_MouseDown);
    //            ItemPalette.DropDown = drop;
    //            return ItemPalette;
    //        }
    //    }
    //    /// <summary>Создание списка графических элементов</summary>
    //    public ToolStripDropDownButton CreateDrawingItemsBox()
    //    {
    //        //Если тепловизиор не активирован - выход
    //        if (!Settings.Thermovision.Enabled.ToBool()) return;
    //        //Определение номера панели тепловизора
    //        int ipanel = 1 - Settings.Interface.TelecameraWindow.ToInt();
    //        ToolStripDropDownButton ItemDrawItems = (ToolStripDropDownButton)ToolStrip[ipanel].Items["ItemDrawItems"];
    //        if (ItemDrawItems == null)
    //        {
    //            Image image = ToolBarImages.Images[(int)ToolbarImagesNames.Pencil];
    //            ItemDrawItems = new ToolStripDropDownButton(image);
    //            ItemDrawItems.Name = "ItemDrawItems";
    //            ItemDrawItems.Alignment = ToolStripItemAlignment.Right;
    //            ItemDrawItems.ToolTipText = "Графические элементы";

    //            ToolStripDropDownMenu drop = new ToolStripDropDownMenu();
    //            //Границы анализа тепловизионного изображения
    //            ToolStripMenuItem item_borders = new ToolStripMenuItem("Границы анализа");
    //            ToolStripMenuItem item_calibr = new ToolStripMenuItem("Границы калибровочного типа");
    //            ToolStripMenuItem item_horiz = new ToolStripMenuItem("Уровень горизонта");
    //            ToolStripMenuItem item_strings = new ToolStripMenuItem("Отображать надписи");
    //            ToolStripMenuItem item_func = new ToolStripMenuItem("Функция распределения температур");
    //            ToolStripMenuItem item_level = new ToolStripMenuItem("Уровень загрузки");
    //            ToolStripMenuItem item_level_doc = new ToolStripMenuItem("Уровень загрузки по документам");
    //            ToolStripMenuItem item_temp = new ToolStripMenuItem("Шкала температур");
    //            ToolStripMenuItem item_show_temp = new ToolStripMenuItem("Сигнал в указанной точке");
    //            ToolStripMenuItem item_check_all = new ToolStripMenuItem("Выбрать все");
    //            ToolStripMenuItem item_uncheck_all = new ToolStripMenuItem("Убрать все");

    //            //События
    //            item_borders.Click += new EventHandler(item_Click);
    //            item_calibr.Click += new EventHandler(item_Click);
    //            item_horiz.Click += new EventHandler(item_Click);
    //            item_strings.Click += new EventHandler(item_Click);
    //            item_func.Click += new EventHandler(item_Click);
    //            item_level.Click += new EventHandler(item_Click);
    //            item_temp.Click += new EventHandler(item_Click);
    //            item_level_doc.Click += new EventHandler(item_Click);
    //            item_show_temp.Click += new EventHandler(item_Click);
    //            item_check_all.Click += new EventHandler(item_Click);
    //            item_uncheck_all.Click += new EventHandler(item_Click);

    //            //Состояние
    //            item_borders.Checked = Settings.ThermoDrawingFlags.FlagDrawBorders.ToBool();
    //            item_calibr.Checked = Settings.ThermoDrawingFlags.FlagDrawCalibre.ToBool();
    //            item_horiz.Checked = Settings.ThermoDrawingFlags.FlagDrawHorizon.ToBool();
    //            item_strings.Checked = Settings.ThermoDrawingFlags.FlagDrawCaption.ToBool();
    //            item_func.Checked = Settings.ThermoDrawingFlags.FlagDrawFunction.ToBool();
    //            item_level.Checked = Settings.ThermoDrawingFlags.FlagDrawLevel.ToBool();
    //            item_level_doc.Checked = Settings.ThermoDrawingFlags.FlagDrawLevelDoc.ToBool();
    //            item_temp.Checked = Settings.ThermoDrawingFlags.FlagDrawScale.ToBool();
    //            item_show_temp.Checked = Settings.ThermoDrawingFlags.FlagDrawTemperature.ToBool();

    //            drop.Items.Add(item_borders);
    //            drop.Items.Add(item_calibr);
    //            drop.Items.Add(item_horiz);
    //            drop.Items.Add(item_strings);
    //            drop.Items.Add(item_func);
    //            drop.Items.Add(item_level);
    //            drop.Items.Add(item_level_doc);
    //            drop.Items.Add(item_temp);
    //            drop.Items.Add(item_show_temp);
    //            drop.Items.Add(item_check_all);
    //            drop.Items.Add(item_uncheck_all);

    //            ItemDrawItems.DropDown = drop;

    //            return ItemDrawItems;
    //        }
    //    }
        
    //}
}
