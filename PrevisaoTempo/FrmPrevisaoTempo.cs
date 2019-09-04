using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace PrevisaoTempo
{
    public partial class FrmPrevisaoTempo : Form
    {
        #region variaveis e obj de classe
        string pasta_aplicacao = Application.StartupPath + @"\"; //Pega endereço da pasta que contem arquivo .exe
        string city_name;
        #endregion

        #region permissões


        #endregion

        #region métodos construtores
        public FrmPrevisaoTempo()
        {
            InitializeComponent();
        }
        #endregion

        #region métodos
        //faz conexão com API
        public void getWeather(string key, string city_name)
        {
            string url = string.Format("https://api.hgbrasil.com/weather?key={0}&city_name={1}&format=json", key, city_name);
            WebClient web = new WebClient();
            var json = web.DownloadString(url);
            byte[] bytes = Encoding.Default.GetBytes(json);
            json = Encoding.UTF8.GetString(bytes);
            var result = JsonConvert.DeserializeObject<WeatherInfo.RootObject>(json);
            WeatherInfo.RootObject outPut = result;

            //convertendo dia da semana para português
            string day = DateTime.Today.DayOfWeek.ToString();
            string dayWeek = "";
            switch (day)
            {
                case "Monday":
                    dayWeek = "Segunda-feira";
                    break;
                case "Tuesday":
                    dayWeek = "Terça-feira";
                    break;
                case "Wednesday":
                    dayWeek = "Quarta-feira";
                    break;
                case "Thursday":
                    dayWeek = "Quinta-feira";
                    break;
                case "Friday":
                    dayWeek = "Sexta-feira";
                    break;
                case "Saturday":
                    dayWeek = "Sábado";
                    break;
                case "Sunday":
                    dayWeek = "Domingo";
                    break;
                default:
                    dayWeek = "ERROR";
                    break;
            }


            //preenchendo campos do form
            city.Text = outPut.results.city_name;
            lbl_forecast0.Text = outPut.results.forecast[0].weekday.ToString();
            lbl_forecast1.Text = outPut.results.forecast[1].weekday.ToString();
            lbl_forecast2.Text = outPut.results.forecast[2].weekday.ToString();
            lbl_forecast3.Text = outPut.results.forecast[3].weekday.ToString();
            lbl_forecast4.Text = outPut.results.forecast[4].weekday.ToString();

            lbl_max0.Text = string.Format("{0}°", outPut.results.forecast[0].max.ToString());
            lbl_max1.Text = string.Format("{0}°", outPut.results.forecast[1].max.ToString());
            lbl_max2.Text = string.Format("{0}°", outPut.results.forecast[2].max.ToString());
            lbl_max3.Text = string.Format("{0}°", outPut.results.forecast[3].max.ToString());
            lbl_max4.Text = string.Format("{0}°", outPut.results.forecast[4].max.ToString());

            lbl_min0.Text = string.Format("{0}°", outPut.results.forecast[0].min.ToString());
            lbl_min1.Text = string.Format("{0}°", outPut.results.forecast[1].min.ToString());
            lbl_min2.Text = string.Format("{0}°", outPut.results.forecast[2].min.ToString());
            lbl_min3.Text = string.Format("{0}°", outPut.results.forecast[3].min.ToString());
            lbl_min4.Text = string.Format("{0}°", outPut.results.forecast[4].min.ToString());

            pictureBox1.Image = Image.FromFile(pasta_aplicacao + @"images\" + outPut.results.forecast[0].condition.ToString() + ".png");
            pictureBox2.Image = Image.FromFile(pasta_aplicacao + @"images\" + outPut.results.forecast[1].condition.ToString() + ".png");
            pictureBox3.Image = Image.FromFile(pasta_aplicacao + @"images\" + outPut.results.forecast[2].condition.ToString() + ".png");
            pictureBox4.Image = Image.FromFile(pasta_aplicacao + @"images\" + outPut.results.forecast[3].condition.ToString() + ".png");
            pictureBox5.Image = Image.FromFile(pasta_aplicacao + @"images\" + outPut.results.forecast[4].condition.ToString() + ".png");
            pictureBox6.Image = Image.FromFile(pasta_aplicacao + @"images\" + outPut.results.condition_slug.ToString() + ".png");

            lbl_day.Text = dayWeek + "  " + outPut.results.time;
            lbl_description.Text = outPut.results.description;
            lbl_temp.Text = outPut.results.temp.ToString();
            lbl_humidity.Text = "Umidade: " + outPut.results.humidity + "%";
            lbl_sunset.Text = outPut.results.sunset;

            //preenchendo tooltip imagens de acordo com descrição da api
            toolTip1.SetToolTip(pictureBox6, outPut.results.description.ToString()); // dia atual
            toolTip1.SetToolTip(pictureBox1, outPut.results.forecast[0].description.ToString());
            toolTip1.SetToolTip(pictureBox2, outPut.results.forecast[1].description.ToString());
            toolTip1.SetToolTip(pictureBox3, outPut.results.forecast[2].description.ToString());
            toolTip1.SetToolTip(pictureBox4, outPut.results.forecast[3].description.ToString());
            toolTip1.SetToolTip(pictureBox5, outPut.results.forecast[4].description.ToString());

        }
        //Passa parâmetros para API após usuario informar nome da cidade
        public void CarregaWeather()
        {
            string key = "21ad83d1";
            while (true)
            {
                try
                {
                    getWeather(key, city_name);
                    break;
                }
                catch (WebException)
                {
                    if (key == "21ad83d1")//primeira chave
                        key = "f6c68068 ";//segunda chave
                    else
                    {
                        MessageBox.Show("Serviço temporariamente indisponível");
                        break;
                    }
                }
            }
        }
        #endregion

        #region eventos de form
        private void PrevisaoTempo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    city_name = txt_cidade.Text;
                    CarregaWeather();
                    txt_cidade.Visible = false;
                    btn_ir.Visible = false;
                }
            }
            if (e.KeyCode == Keys.F1)
            {
                txt_cidade.Visible = true;
                btn_ir.Visible = true;
                txt_cidade.Focus();
            }
        }

        private void btn_ir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_cidade.Text))
            {
                city_name = txt_cidade.Text;
                CarregaWeather();
                txt_cidade.Visible = false;
                btn_ir.Visible = false;
            }
        }
        #endregion
        

    }

}


