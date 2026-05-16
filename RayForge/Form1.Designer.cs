namespace RayForge
{
    partial class Form1
    {
        /// <summary>
        /// Змінна дизайнера для керування компонентами.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Звільнення ресурсів (пам'яті), які використовує форма.
        /// </summary>
        /// <param name="disposing">true, якщо керовані ресурси мають бути видалені; інакше — false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Метод для ініціалізації компонентів вікна. 
        /// Його не варто редагувати вручну, оскільки зміни можуть бути перезаписані дизайнером.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout(); 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(298, 184);
            MaximizeBox = false;
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false); 
        }
        #endregion
    }
}