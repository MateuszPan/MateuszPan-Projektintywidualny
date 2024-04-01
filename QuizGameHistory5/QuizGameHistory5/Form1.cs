using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QuizGameHistory5
{
    public partial class Form1 : Form
    {
        private Panel welcomePanel;
        private Panel preWelcomePanel;
        private string backgroundImagePath = @"C:\Users\User\QuizGameHistory5\QuizGameHistory5\bin\Debug\net8.0-windows\Resources\tlo_epoka1.jpg";
        private string questionBackgroundImagePath = @"C:\Users\User\QuizGameHistory5\QuizGameHistory5\bin\Debug\net8.0-windows\Resources\tlo_pytan_mapa.jpg";

        private Panel questionPanel;
        private Button[] answerButtons;

        private List<string[]> questionsAndAnswersList;
        private int currentQuestionIndex = 0;

        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializePreWelcomePanel();
        }

        private void InitializePreWelcomePanel()
        {
            preWelcomePanel = new Panel();
            preWelcomePanel.Size = new Size(400, 500);
            preWelcomePanel.BackColor = Color.Transparent;
            preWelcomePanel.BackgroundImage = Image.FromFile(backgroundImagePath);
            preWelcomePanel.BackgroundImageLayout = ImageLayout.Stretch;

            preWelcomePanel.Location = new Point((this.Width - preWelcomePanel.Width) / 2, (this.Height - preWelcomePanel.Height) / 2);

            Label preWelcomeTitleLabel = new Label();
            preWelcomeTitleLabel.Text = "Sprawdź swoją wiedzę historyczną";
            preWelcomeTitleLabel.Font = new Font(preWelcomeTitleLabel.Font, FontStyle.Bold);
            preWelcomeTitleLabel.AutoSize = true;
            preWelcomeTitleLabel.Location = new Point((preWelcomePanel.Width - preWelcomeTitleLabel.Width) / 2 - 40, 140);
            preWelcomePanel.Controls.Add(preWelcomeTitleLabel);

            Button playButton = new Button();
            playButton.Size = new Size(200, 50);
            playButton.Text = "Chcesz zagrać? Naciśnij mnie";
            playButton.Location = new Point((preWelcomePanel.Width - playButton.Width) / 2, 200);
            playButton.Click += (sender, e) => ShowWelcomePanel();
            preWelcomePanel.Controls.Add(playButton);

            this.Controls.Add(preWelcomePanel);
        }

        private void ShowWelcomePanel()
        {
            preWelcomePanel.Visible = false;
            InitializeWelcomePanel();
        }

        private void InitializeWelcomePanel()
        {
            welcomePanel = new Panel();
            welcomePanel.Size = new Size(400, 500);
            welcomePanel.BackColor = Color.Transparent;
            welcomePanel.BackgroundImage = Image.FromFile(backgroundImagePath);
            welcomePanel.BackgroundImageLayout = ImageLayout.Stretch;

            welcomePanel.Location = new Point((this.Width - welcomePanel.Width) / 2, (this.Height - welcomePanel.Height) / 2);

            Label titleLabel = new Label();
            titleLabel.Text = "Wybierz epokę:";
            titleLabel.Font = new Font(titleLabel.Font, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point((welcomePanel.Width - titleLabel.Width) / 2, 110);
            welcomePanel.Controls.Add(titleLabel);

            AddEpochButton("Starożytność", "starozytnosc", 140);
            AddEpochButton("Średniowiecze", "sredniowiecze", 190);
            AddEpochButton("Nowożytność", "nowozytnosc", 240);
            AddEpochButton("Współczesność", "wspolczesnosc", 290);

            this.Controls.Add(welcomePanel);
        }

        private void AddEpochButton(string text, string epochName, int yOffset)
        {
            Button epochButton = new Button();
            epochButton.Size = new Size(200, 50);
            epochButton.Text = text;
            epochButton.Location = new Point((welcomePanel.Width - epochButton.Width) / 2, yOffset);
            epochButton.Click += (sender, e) => EpochButton_Click(sender, e, epochName);
            welcomePanel.Controls.Add(epochButton);
        }

        private void EpochButton_Click(object sender, EventArgs e, string epochName)
        {
            string selectedFile = Path.Combine(Application.StartupPath, "Resources", $"{epochName}.csv");
            LoadQuestionsFromFile(selectedFile);
            InitializeQuestionPanel();
            welcomePanel.Visible = false;
        }

        private void InitializeQuestionPanel()
        {
            questionPanel = new Panel();
            questionPanel.Size = new Size(600, 400);
            questionPanel.BackColor = Color.Transparent;
            questionPanel.BackgroundImage = Image.FromFile(questionBackgroundImagePath);
            questionPanel.BackgroundImageLayout = ImageLayout.Stretch;
            questionPanel.BorderStyle = BorderStyle.FixedSingle;
            questionPanel.Location = new Point((this.Width - questionPanel.Width) / 2, (this.Height - questionPanel.Height) / 2);

            Panel questionRectangle = new Panel();
            questionRectangle.Size = new Size(500, 50);
            questionRectangle.BackColor = Color.LightGreen;
            questionRectangle.BorderStyle = BorderStyle.FixedSingle;
            questionRectangle.Location = new Point((questionPanel.Width - questionRectangle.Width) / 2, 50);
            questionPanel.Controls.Add(questionRectangle);

            Random random = new Random();
            currentQuestionIndex = random.Next(0, questionsAndAnswersList.Count);

            Label questionLabel = new Label();
            questionLabel.Text = questionsAndAnswersList[currentQuestionIndex][0];
            questionLabel.AutoSize = false;
            questionLabel.TextAlign = ContentAlignment.MiddleCenter;
            questionLabel.Font = new Font(questionLabel.Font, FontStyle.Bold);
            questionLabel.Dock = DockStyle.Fill;
            questionRectangle.Controls.Add(questionLabel);

            answerButtons = new Button[4];
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i] = new Button();
                answerButtons[i].Size = new Size(400, 30);
                answerButtons[i].Text = questionsAndAnswersList[currentQuestionIndex][i + 1].Trim(';');
                answerButtons[i].Location = new Point((questionPanel.Width - answerButtons[i].Width) / 2, questionRectangle.Location.Y + questionRectangle.Height + 30 + i * 40);
                answerButtons[i].Click += (sender, e) => AnswerButton_Click(sender, e, answerButtons[i].Text);
                questionPanel.Controls.Add(answerButtons[i]);
            }

            this.Controls.Add(questionPanel);
        }

        private void LoadQuestionsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Plik {filePath} nie istnieje.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();

                questionsAndAnswersList = new List<string[]>();

                foreach (var line in lines)
                {
                    string[] parts = line.Split(',');
                    questionsAndAnswersList.Add(parts);
                }

                currentQuestionIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas wczytywania pytań z pliku: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void AnswerButton_Click(object sender, EventArgs e, string answer)
        {
            MessageBox.Show($"Wybrałeś odpowiedź: {answer}", "Wynik", MessageBoxButtons.OK, MessageBoxIcon.Information);
            currentQuestionIndex++;
            if (currentQuestionIndex < questionsAndAnswersList.Count)
            {
                RefreshQuestionPanel();
            }
            else
            {
                MessageBox.Show("Koniec gry. Gratulacje!", "Koniec gry", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshQuestionPanel()
        {
            questionPanel.Controls.Clear();

            Panel questionRectangle = new Panel();
            questionRectangle.Size = new Size(700, 100);
            questionRectangle.BackColor = Color.LightGreen;
            questionRectangle.BorderStyle = BorderStyle.FixedSingle;
            questionRectangle.Location = new Point((questionPanel.Width - questionRectangle.Width) / 2, (questionPanel.Height - questionRectangle.Height) / 2 - 50);
            questionPanel.Controls.Add(questionRectangle);

            Random random = new Random();
            currentQuestionIndex = random.Next(0, questionsAndAnswersList.Count);

            Label questionLabel = new Label();
            questionLabel.Text = questionsAndAnswersList[currentQuestionIndex][0];
            questionLabel.AutoSize = false;
            questionLabel.TextAlign = ContentAlignment.MiddleCenter;
            questionLabel.Size = new Size(480, 40);
            questionRectangle.Controls.Add(questionLabel);

            answerButtons = new Button[4];
            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i] = new Button();
                answerButtons[i].Size = new Size(700, 50);
                answerButtons[i].Text = questionsAndAnswersList[currentQuestionIndex][i + 1];
                answerButtons[i].Location = new Point((questionPanel.Width - answerButtons[i].Width) / 2, questionRectangle.Location.Y + questionRectangle.Height + 30 + i * 70);
                answerButtons[i].Click += (sender, e) => AnswerButton_Click(sender, e, answerButtons[i].Text);
                questionPanel.Controls.Add(answerButtons[i]);
            }

            this.Controls.Add(questionPanel);
        }
    }
}