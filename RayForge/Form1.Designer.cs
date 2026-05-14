namespace RayForge
{
    // partial означає, що цей клас є лише частиною Form1. 
    // Основна логіка зазвичай знаходиться у файлі Form1.cs.
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
            SuspendLayout(); // Тимчасово призупиняє логіку макетування для прискорення завантаження
            
            // 
            // Налаштування Form1
            // 
            
            // Масштабування залежно від шрифту системи
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            
            // Початковий розмір робочої області вікна (298 на 184 пікселів)
            ClientSize = new Size(298, 184);
            
            // Вимикає кнопку розгортання вікна на весь екран
            MaximizeBox = false;
            
            // Внутрішнє ім'я об'єкта та текст у заголовку вікна
            Name = "Form1";
            Text = "Form1";
            
            ResumeLayout(false); // Відновлює логіку макетування
        }

        #endregion
    }
}