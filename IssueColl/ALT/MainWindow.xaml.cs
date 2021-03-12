﻿////using Atlassian.Jira;
////using Jiracoll.ReportBuilder;
////using Newtonsoft.Json;
////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Linq;

////namespace Jiracoll
////{
////    /// <summary>
////    /// Interaktionslogik für MainWindow.xaml
////    /// </summary>
////    public partial class MainWindow : Window
////    {
////        public MainWindow()
////        {
////            InitializeComponent();
////            TextBlock_Filepath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\test1234.csv";
////            TextBlock_WorkflowFilePath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\workflow.txt";
////            DatePicker_DateOfFile.SelectedDate = DateTime.Now;
////        }
////        // Aufbau CFD CSV mit API Call
////        private async void Button_ReadFromAPI(object sender, RoutedEventArgs e)
////        {
////            string usr = TextBox_User.Text;
////            string pw = PasswordBox_pw.Password;
////            string url = TextBox_Server_URL.Text;

////            var jira = Jira.CreateRestClient(url, usr, pw);

////            Console.WriteLine(jira.ToString());

////            String projectKey = TextBox_ProjectKey.Text;
////            //string jqlLine = "project = " + projectKey;
////            string jqlLine = TextBox_ProjectKey.Text;

////            //Project p = await jira.Projects.GetProjectAsync(projectKey);
////            String csvFileContent = "";
////            csvFileContent += "Key,Issuetype,Current Status,Created Date,";

////            var s = await jira.Statuses.GetStatusesAsync();
////            foreach (IssueStatus issueStatus in s)
////            {
////                csvFileContent += issueStatus.ToString() + ",";
////            }
////            csvFileContent += System.Environment.NewLine;

////            int counter = 0; // Verlaufsbalken Zähler
////            int startAt = 0; // Paginierter Abruf bei Jira (immer 20 Items), startpunkt
////            int issuesCount; // wieviele Issues insgesamt issuecount/20 == anzahl abrufe notwendig

////            var issues = await jira.Issues.GetIssuesFromJqlAsync(jqlLine);

////            issuesCount = issues.TotalItems + 30; //zur sicherheit :-)
////            startAt += 20;

////            while (startAt < issuesCount)
////            {
////                issues = await jira.Issues.GetIssuesFromJqlAsync(jqlLine, startAt);
////                startAt += 20;
////            }

////            ProgressBar_Historie.Value = 100 / issues.Count() * counter;

////            // suche pro abgerufenes Issue die Basisdaten..... 
////            foreach (Issue i in issues)
////            {
////                String resultLine = "";
////                resultLine += i.Key + "," + i.Type + "," + i.Status + "," + i.Created + ",";

////                Dictionary<string, string> dict = new Dictionary<string, string>();
////                foreach (IssueStatus issueStatus in s)
////                {
////                    dict.Add(issueStatus.ToString(), "");
////                }

////                //  .... und durchlaufe das Changelog jedes Issues und suche nach Statuswechseln
////                // ACHTUNG! Imho ergänzt er mom wahrscheinlich um den LETZTEN Statuswechsel. Je nachdem wie das Log aufgebaut ist
////                // TODO mal prüfen, d.h. testcase basteln
////                // Für mehr genauigkeit ist hier mehr Arbeit notwendig
////                // i.g. => Suche Alle Satuswechsel, sortiere sie  nach Zeitverlauf und summiere dann die einzelnen Zeiten. 
////                // TODO genau so nen collector bauen
////                IEnumerable<IssueChangeLog> logs = await i.GetChangeLogsAsync();

////                foreach (IssueChangeLog log in logs)
////                {
////                    foreach (IssueChangeLogItem item in log.Items)
////                    {
////                        if (item.FieldName.Equals("status"))
////                        {
////                            dict[item.ToValue] = log.CreatedDate.ToShortDateString();

////                        }
////                    }
////                }
////                foreach (KeyValuePair<string, string> pair in dict)
////                {
////                    resultLine += pair.Value + ",";
////                }

////                csvFileContent += resultLine + System.Environment.NewLine;
////                Console.WriteLine(resultLine);
////                counter++;
////                ProgressBar_Historie.Value = 100 / issues.Count() * counter;

////            }
////            File.WriteAllText(TextBlock_Filepath.Text, csvFileContent);
////            ProgressBar_Historie.Value = 100;

////        }

////        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
////        {

////        }

////        // CSV export pfad
////        private void Button_FilePath_Click(object sender, RoutedEventArgs e)
////        {
////            SaveFileDialog saveFileDialog = new SaveFileDialog();
////            saveFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
////            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
////            if (saveFileDialog.ShowDialog() == true)
////                TextBlock_Filepath.Text = saveFileDialog.FileName;
////            File.WriteAllText(saveFileDialog.FileName, "");
////        }



////        // Auswahl der einzulesenden Json Datei
////        private void Button_CfdFromJson_Click(object sender, RoutedEventArgs e)
////        {

////            CFDDataFileBuilder builder = new CFDDataFileBuilder();
////            string workflowPath = TextBlock_WorkflowFilePath.Text;
////            string jsonpath = TextBlock_FilePathJson.Text;
////            DateTime timestamp = (DateTime)DatePicker_DateOfFile.SelectedDate;

////            builder.buildCFDDataFile(workflowPath, jsonpath, timestamp);

////        }



////        /* Aufbau einer Tabelle: pro gefundenem Issue aufsummiert alle Zeiten pro Status in Minuten.
////        e.g.   To Do  | In Progress | In Test
////               500    |   876       |  456
////        gezählt werden Verstreichminuten (Progress Time), NICHT action Time (echte Arbeitszeit)
////        24 h / 7 Tage Woche 
////        */
////        private void Button_IssuesFromJson(object sender, RoutedEventArgs e)
////        {
////            string jsonString = "";
////            int counter = 0; // Verlaufsbalken Zähler

////            int issuesCount; // wieviele Issues insgesamt issuecount/20 == anzahl abrufe notwendig

////            String csvFileContent = "";
////            WorkflowExtraxtor wfex = new WorkflowExtraxtor();
////            IssueChangeLog issueChangelog = new IssueChangeLog();
////            List<String> notFoundStep = new List<String>();

////            if (true)
////            {
////                // get json & deserialize

////                jsonString = File.ReadAllText(TextBlock_FilePathJson.Text);

////                TextBox_JsonPath.Text = TextBlock_FilePathJson.Text;

////                IssuesPOCO JsonContent = JsonConvert.DeserializeObject<IssuesPOCO>(jsonString);

////                csvFileContent = "";

////                csvFileContent += "Group,Key,Issuetype,Status,Created Date,Component,Resolution,";

////                List<WorkflowStep> statuses = wfex.getWorkflowFromFile(TextBlock_WorkflowFilePath.Text);

////                List<string> doneStatesList = new List<string>();

//// 
////foreach (WorkflowStep status in statuses)
////{
////    csvFileContent += status.Name + ",";
////    if (status.DoneState)
////    {
////        doneStatesList.Add(status.Name);
////    }
////}

////csvFileContent += "First Date,Closed Date";

////csvFileContent += System.Environment.NewLine;

//// baue dictionary mit status/zeitpaaren
////issuesCount = JsonContent.issues.Count;

////foreach (IssuePOCO issue in JsonContent.issues)
////{
////    String resultLine = "";
////    string lastName = "";
////    string firstName = "";
////    string group = "";
////    Boolean foundDate = false;
////    // json convert anpassen auf issuetype "Fields hinzufügen"

////    resultLine += group + "," + issue.key + "," + issue.fields.issuetype.name + "," + issue.fields.status.name + "," + issue.fields.created.ToString() + ",";

////    if (issue.fields.components != null)
////    {
////        foreach (IssueComponentsItemPOCO item in issue.fields.components)
////        {
////            resultLine += item.name + "|";
////        }

////    }
////    resultLine += ",";

////    if (issue.fields.resolution != null)
////    {
////        resultLine += issue.fields.resolution.name;
////    }
////    resultLine += ",";


//    Dictionary<string, int> dict = new Dictionary<string, int>();
//    foreach (WorkflowStep status in statuses)
//    {
//        dict[status.Name] = 0;
//        if (status.Last)
//        {
//            lastName = status.Name;
//        }
//        if (status.First)
//        {
//            firstName = status.Name;
//        }
//    }

//    List<StatusRich> statusRichList = new List<StatusRich>();

//    foreach (IssueHistoryPOCO history in issue.changelog.histories)
//    {
//        foreach (IssueChangeLogItem item in history.items)
//        {
//            if (item.FieldName.Equals("status"))
//            {
//                StatusRich statusTransformation = new StatusRich(item.ToValue, DateTime.Parse(history.created.ToString()));

//                statusRichList.Add(statusTransformation);
//            }
//        }
//    }
//    DateTime CloseDate = new DateTime();
//    DateTime FirstDate = new DateTime();
//    DateTime DoneDate = new DateTime();
//    // umsortieren letzter zuerst, desc
//    statusRichList.Sort((x, y) => y.TimeStamp.CompareTo(x.TimeStamp));
//    DateTime currentDate = (DateTime)DatePicker_DateOfFile.SelectedDate;
//    // kein Statuswechsel in History ==> immer noch im initialen Status
//    if (statusRichList.Count < 1)
//    {
//        //DateTime currentDate = new DateTime(2020, 12, 17, 12, 29, 00);

//        TimeSpan ts = currentDate - issue.fields.created;
//        int minutes = (int)ts.TotalMinutes;

//        resultLine += minutes + ",,,,,,,,";
//    }
//    // sonst Status gefunden, wenn  nicht: immer noch open
//    else
//    {
//        DateTime last;

//        // wenn es einen Donestatus gibt ist der letzte das Ende Date
//        //if (statusRichList.Any(p => p.Name == "Done") || statusRichList.Any(p => p.Name == "Abgebrochen"))
//        if (statusRichList.Any(p => p.Name.Equals(lastName)))
//        {
//            CloseDate = statusRichList.Max(obj => obj.TimeStamp);
//        }

//        if (statusRichList.Any(p => p.Name.Equals(firstName)))
//        {
//            FirstDate = statusRichList.Min(obj => obj.TimeStamp);
//        }

//        // Erster Zeitpunkt: Erstelldatum des Datenabzugs (aka "heute")                                                
//        last = currentDate;
//        // Dauer eines statusverbleibs: Startdate des nachfolgers - Startdate des betrachteten Status
//        foreach (StatusRich statusTrans in statusRichList)
//        {
//            TimeSpan ts = last - statusTrans.TimeStamp;
//            statusTrans.Minutes = (int)ts.TotalMinutes;
//            last = statusTrans.TimeStamp;
//            string statusName = "";
//            if (!(dict.ContainsKey(statusTrans.Name)))
//            {
//                statusName = statusTrans.Name;
//                foreach (WorkflowStep step in statuses)
//                {
//                    if (step.Aliases.Contains(statusTrans.Name))
//                    {
//                        statusName = step.Name;
//                    }
//                }
//            }
//            else
//            {
//                statusName = statusTrans.Name;
//            }
//            if (doneStatesList.Contains(statusName))
//            {
//                DoneDate = statusTrans.TimeStamp;
//                foundDate = true;
//            }
//            if (!dict.ContainsKey(statusName))
//            {
//                dict.Add(statusName, 0);
//                if (!notFoundStep.Contains(statusName))
//                {
//                    notFoundStep.Add(statusName);
//                }
//            }
//            dict[statusName] += statusTrans.Minutes;

//        }
//        foreach (KeyValuePair<string, int> pair in dict)
//        {
//            resultLine += pair.Value + ",";
//        }
//    }



//    if (FirstDate.Equals(new DateTime()))
//    {
//        resultLine += ",";
//    }
//    else
//    {
//        resultLine += FirstDate.ToString() + ",";
//    }

//    if (CloseDate.Equals(new DateTime()))
//    {
//        if (foundDate)
//        {
//            resultLine += DoneDate.ToString() + ",";
//        }
//        /*resultLine += ",";*/
//    }
//    else
//    {
//        resultLine += CloseDate.ToString();
//    }

//    csvFileContent += resultLine + System.Environment.NewLine;

//    Console.WriteLine(resultLine);
//    counter++;
//    ProgressBar_Historie.Value = 100 / issuesCount * counter;
//}

//            }

//            TextBox_Errors.Text += "\n<< additional Statuses found >> \n";
//foreach (String s in notFoundStep)
//{
//    TextBox_Errors.Text += s + " \n";
//}

//File.WriteAllText(TextBlock_Filepath.Text, csvFileContent);
//        }

//        private void ProgressBar_Historie_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
//{
//}

////private void Button_JsonFile_Click(object sender, RoutedEventArgs e)
////{
////    OpenFileDialog openFileDialog = new OpenFileDialog();
////    openFileDialog.Filter = "json File (*.json|*.json|All files (*.*)|*.*";
////    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
////    if (openFileDialog.ShowDialog() == true)
////        TextBlock_FilePathJson.Text = openFileDialog.FileName;
////}

////private void Button_workflowHistory_Click(object sender, RoutedEventArgs e)
////{

////}

////private void Button_SelectWorkflowFile_Click(object sender, RoutedEventArgs e)
////{
////    OpenFileDialog openFileDialog = new OpenFileDialog();
////    openFileDialog.Filter = "txt File (*.txt|*.txt|All files (*.*)|*.*";
////    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
////    if (openFileDialog.ShowDialog() == true)
////        TextBlock_WorkflowFilePath.Text = openFileDialog.FileName;
////}
////    }
////}
