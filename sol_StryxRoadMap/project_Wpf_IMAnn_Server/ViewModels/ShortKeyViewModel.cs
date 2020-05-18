using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using System.Windows.Controls;
using System.Windows;
using Wpf_IMAnn_Server.Utils;
using Wpf_IMAnn_Server.Views;

namespace Wpf_IMAnn_Server.ViewModels
{
    public class ShortKeyViewModel : Screen
    {
        public ShortKeyViewModel()
        {
            
        }

        public BindableCollection<string> FieldComboBox_1 { get; set; }
        public BindableCollection<string> FieldComboBox_2 { get; set; }
        public BindableCollection<string> FieldComboBox_3 { get; set; }
        public BindableCollection<string> FieldComboBox_4 { get; set; }

        List<string> comboboxList;

        string item_1, item_2, item_3, item_4;
        string[] ShortKeyList;

        public void SignFieldSelected1(object item)
        {
            item_1 = item.ToString();
        }
        public void SignFieldSelected2(object item)
        {
            item_2 = item.ToString();
        }
        public void SignFieldSelected3(object item)
        {
            item_3 = item.ToString();
        }
        public void SignFieldSelected4(object item)
        {
            item_4 = item.ToString();
        }

        public void ShortKeyOk()
        {
            ShortKeyList = new string[] { item_1, item_2, item_3, item_4 };
            EventAggregationProvider.EventAggregator.PublishOnUIThread(ShortKeyList);
            ShortKeyView ShortKeyView = (ShortKeyView)GetView();
            Window.GetWindow(ShortKeyView).Close();
        }

        public void CategorySelected(RoutedEventArgs routedEvent)
        {
            Console.WriteLine(routedEvent.Source);
            switch (((ComboBoxItem)routedEvent.Source).Tag)
            {
                case "1_101":
                    comboboxList = Enum.GetValues(typeof(SignField_Caution)).Cast<SignField_Caution>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);                    
                    break;

                case "1_102":
                    comboboxList = Enum.GetValues(typeof(SignField_Regulation)).Cast<SignField_Regulation>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_103":
                    comboboxList = Enum.GetValues(typeof(SignField_Instruction)).Cast<SignField_Instruction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_104":
                    comboboxList = Enum.GetValues(typeof(SignField_Assistant)).Cast<SignField_Assistant>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_201":
                    comboboxList = Enum.GetValues(typeof(SurfaceMark)).Cast<SurfaceMark>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_301":
                    comboboxList = Enum.GetValues(typeof(LightField)).Cast<LightField>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_401":
                    comboboxList = Enum.GetValues(typeof(Human)).Cast<Human>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_402":
                    comboboxList = Enum.GetValues(typeof(Car)).Cast<Car>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_501":
                    comboboxList = Enum.GetValues(typeof(Construction)).Cast<Construction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;

                case "1_000":
                    comboboxList= Enum.GetValues(typeof(SignField_ETC)).Cast<SignField_ETC>().Select(v => v.ToString()).ToList();
                    FieldComboBox_1 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_1);
                    break;


                    
                case "2_101":
                    comboboxList = Enum.GetValues(typeof(SignField_Caution)).Cast<SignField_Caution>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_102":
                    comboboxList = Enum.GetValues(typeof(SignField_Regulation)).Cast<SignField_Regulation>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_103":
                    comboboxList = Enum.GetValues(typeof(SignField_Instruction)).Cast<SignField_Instruction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_104":
                    comboboxList = Enum.GetValues(typeof(SignField_Assistant)).Cast<SignField_Assistant>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_201":
                    comboboxList = Enum.GetValues(typeof(SurfaceMark)).Cast<SurfaceMark>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_301":
                    comboboxList = Enum.GetValues(typeof(LightField)).Cast<LightField>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_401":
                    comboboxList = Enum.GetValues(typeof(Human)).Cast<Human>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_402":
                    comboboxList = Enum.GetValues(typeof(Car)).Cast<Car>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_501":
                    comboboxList = Enum.GetValues(typeof(Construction)).Cast<Construction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2= new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;

                case "2_000":
                    comboboxList = Enum.GetValues(typeof(SignField_ETC)).Cast<SignField_ETC>().Select(v => v.ToString()).ToList();
                    FieldComboBox_2 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_2);
                    break;



                case "3_101":
                    comboboxList = Enum.GetValues(typeof(SignField_Caution)).Cast<SignField_Caution>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_102":
                    comboboxList = Enum.GetValues(typeof(SignField_Regulation)).Cast<SignField_Regulation>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_103":
                    comboboxList = Enum.GetValues(typeof(SignField_Instruction)).Cast<SignField_Instruction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_104":
                    comboboxList = Enum.GetValues(typeof(SignField_Assistant)).Cast<SignField_Assistant>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_201":
                    comboboxList = Enum.GetValues(typeof(SurfaceMark)).Cast<SurfaceMark>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_301":
                    comboboxList = Enum.GetValues(typeof(LightField)).Cast<LightField>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_401":
                    comboboxList = Enum.GetValues(typeof(Human)).Cast<Human>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_402":
                    comboboxList = Enum.GetValues(typeof(Car)).Cast<Car>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_501":
                    comboboxList = Enum.GetValues(typeof(Construction)).Cast<Construction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;

                case "3_000":
                    comboboxList = Enum.GetValues(typeof(SignField_ETC)).Cast<SignField_ETC>().Select(v => v.ToString()).ToList();
                    FieldComboBox_3 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_3);
                    break;



                case "4_101":
                    comboboxList = Enum.GetValues(typeof(SignField_Caution)).Cast<SignField_Caution>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_102":
                    comboboxList = Enum.GetValues(typeof(SignField_Regulation)).Cast<SignField_Regulation>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_103":
                    comboboxList = Enum.GetValues(typeof(SignField_Instruction)).Cast<SignField_Instruction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_104":
                    comboboxList = Enum.GetValues(typeof(SignField_Assistant)).Cast<SignField_Assistant>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_201":
                    comboboxList = Enum.GetValues(typeof(SurfaceMark)).Cast<SurfaceMark>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_301":
                    comboboxList = Enum.GetValues(typeof(LightField)).Cast<LightField>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_401":
                    comboboxList = Enum.GetValues(typeof(Human)).Cast<Human>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_402":
                    comboboxList = Enum.GetValues(typeof(Car)).Cast<Car>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_501":
                    comboboxList = Enum.GetValues(typeof(Construction)).Cast<Construction>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                case "4_000":
                    comboboxList = Enum.GetValues(typeof(SignField_ETC)).Cast<SignField_ETC>().Select(v => v.ToString()).ToList();
                    FieldComboBox_4 = new BindableCollection<string>(comboboxList);
                    NotifyOfPropertyChange(() => FieldComboBox_4);
                    break;

                default:
                    break;
            }
        }

    }
}
