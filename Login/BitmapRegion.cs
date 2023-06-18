using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


/// 
/// Summary description for BitmapRegion. 
/// 
public class BitmapRegion
{
    public BitmapRegion() {}


    /// 
    /// Create and apply the region on the supplied control
    /// 創建支持位圖區域的控件（目前有button和form）
    /// 
    /// The Control object to apply the region to控件 
    /// The Bitmap object to create the region from位圖 
    public static void CreateControlRegion(Control control, Bitmap bitmap)
    {
        // Return if control and bitmap are null
        //判斷是否存在控件和位圖
        if (control == null || bitmap == null)
            return;

        // Set our control''s size to be the same as the bitmap
        //設置控件大小為位圖大小
        control.Width = bitmap.Width;
        control.Height = bitmap.Height;
        // Check if we are dealing with Form here 
        //當控件是form時
        if (control is System.Windows.Forms.Form)
        {
            // Cast to a Form object
            //強制轉換為FORM
            Form form = (Form)control;
            // Set our form''s size to be a little larger that the  bitmap just 
            // in case the form''s border style is not set to none in the first place 
            //當FORM的邊界FormBorderStyle不為NONE時，應將FORM的大小設置成比位圖大小稍大一點
            form.Width = control.Width;
            form.Height = control.Height;
            // No border 
            //沒有邊界
            form.FormBorderStyle = FormBorderStyle.None;
            // Set bitmap as the background image 
            //將位圖設置成窗體背景圖片
            form.BackgroundImage = bitmap;
            // Calculate the graphics path based on the bitmap supplied 
            //計算位圖中不透明部分的邊界
            GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
            // Apply new region 
            //應用新的區域
            form.Region = new Region(graphicsPath);
        }
        // Check if we are dealing with Button here 
        //當控件是button時
        else if (control is System.Windows.Forms.Button)
        {
            // Cast to a button object 
            //強制轉換為 button
            Button button = (Button)control;
            // Do not show button text 
            //不顯示button text
            button.Text = "";

            // Change cursor to hand when over button 
            //改變 cursor的style
            button.Cursor = Cursors.Hand;
            // Set background image of button 
            //設置button的背景圖片
            button.BackgroundImage = bitmap;

            // Calculate the graphics path based on the bitmap supplied 
            //計算位圖中不透明部分的邊界
            GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
            // Apply new region 
            //應用新的區域
            button.Region = new Region(graphicsPath);
        }
    }
    /// 
    /// Calculate the graphics path that representing the figure in the bitmap 
    /// excluding the transparent color which is the top left pixel. 
    /// //計算位圖中不透明部分的邊界
    /// 
    /// The Bitmap object to calculate our graphics path from 
    /// Calculated graphics path 
    private static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap)
    {
        // Create GraphicsPath for our bitmap calculation 
        //創建 GraphicsPath
        GraphicsPath graphicsPath = new GraphicsPath();
        // Use the top left pixel as our transparent color 
        //使用左上角的一點的顏色作為我們透明色
        Color colorTransparent = bitmap.GetPixel(0, 0);
        // This is to store the column value where an opaque pixel is first found. 
        // This value will determine where we start scanning for trailing opaque pixels.
        //第一個找到點的X
        int colOpaquePixel = 0;
        // Go through all rows (Y axis) 
        // 偏歷所有行（Y方向）
        for (int row = 0; row < bitmap.Height; row++)
        {
            // Reset value 
            //重設
            colOpaquePixel = 0;
            // Go through all columns (X axis) 
            //偏歷所有列（X方向）
            for (int col = 0; col < bitmap.Width; col++)
            {
                // If this is an opaque pixel, mark it and search for anymore trailing behind 
                //如果是不需要透明處理的點則標記，然後繼續偏歷
                if (bitmap.GetPixel(col, row) != colorTransparent)
                {
                    // Opaque pixel found, mark current position
                    //記錄當前
                    colOpaquePixel = col;
                    // Create another variable to set the current pixel position 
                    //建立新變量來記錄當前點
                    int colNext = col;
                    // Starting from current found opaque pixel, search for anymore opaque pixels 
                    // trailing behind, until a transparent   pixel is found or minimum width is reached 
                    ///從找到的不透明點開始，繼續尋找不透明點,一直到找到或則達到圖片寬度 
                    for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
                        if (bitmap.GetPixel(colNext, row) == colorTransparent)
                            break;
                    // Form a rectangle for line of opaque   pixels found and add it to our graphics path 
                    //將不透明點加到graphics path
                    graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                    // No need to scan the line of opaque pixels just found 
                    col = colNext;
                }
            }
        }
        // Return calculated graphics path 
        return graphicsPath;
    }
}
