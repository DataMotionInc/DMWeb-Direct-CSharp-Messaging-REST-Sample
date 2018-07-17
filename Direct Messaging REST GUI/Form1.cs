using System;
using System.Windows.Forms;
using System.IO;
using Direct_Messaging_SDK_461;
using Direct_Messaging_SDK_461.Models;
using System.Text.RegularExpressions;

namespace Direct_Messaging_REST_GUI
{
    public partial class Form1 : Form
    {
        DM_DirectMessaging_461 dmWeb = new DM_DirectMessaging_461();

        public Form1()
        {
            InitializeComponent();
        }

        private void StartWaitCursor()
        {
            this.UseWaitCursor = true;
            Application.DoEvents();
        }

        private void EndWaitCursor()
        {
            this.UseWaitCursor = false;
            Cursor.Current = Cursors.Default;
        }
        // This button passes the user's email and password to the base URL of their choice to obtain a Session Key
        private async void logOnButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            Account.LogOn user = new Account.LogOn();
            string response = "";

            user.UserIdOrEmail = usernameTextBox.Text;
            user.Password = passwordTextBox.Text;

            string baseUrl = baseUrlTextBox.Text;
            if (baseUrl == "")
            {
                dmWeb = new DM_DirectMessaging_461();
            }
            else
            {
                dmWeb = new DM_DirectMessaging_461(baseUrl);
            }

            if (user.UserIdOrEmail != "" && user.Password != "")
            {
                try
                {
                    response = await dmWeb.Account.LogOn(user);
                    accountTextBox.Text = string.Format("Session Key: {0}", response);

                    usernameTextBox.Text = "";
                    passwordTextBox.Text = "";
                    logOnButton.Enabled = false;
                    accountDetails.Enabled = true;
                    logoutButton.Enabled = true;
                    passwordChangeButton.Enabled = true;

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    accountTextBox.Text = "Invalid credentials";
                    usernameTextBox.Text = "";
                    passwordTextBox.Text = "";

                    EndWaitCursor();
                    return;
                }
            }
            else
            {
                accountTextBox.Text = "UserIdOrEmail and/or Password are missing";

                this.UseWaitCursor = false;
                Cursor.Current = Cursors.Default;
            }
        }

        // This button logs out of the user's account
        private async void logoutButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            string response = "";

            try
            {
                response = await dmWeb.Account.LogOut();
                accountTextBox.Text = "Log Out successful";

                EndWaitCursor();
            }
            catch (Exception ex)
            {
                accountTextBox.Text = ex.Message;

                EndWaitCursor();
            }

            logOnButton.Enabled = true;
            accountDetails.Enabled = false;
            passwordChangeButton.Enabled = false;
            logoutButton.Enabled = false;
        }

        // This button receives a response of type AccountResponses which is displayed
        public async void accountDetails_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            Account.AccountDetails response = new Account.AccountDetails();

            try
            {
                response = await dmWeb.Account.Details();

                EndWaitCursor();
            }
            catch (Exception ex)
            {
                accountTextBox.Text = ex.Message;

                EndWaitCursor();
                return;
            }

            accountTextBox.Text = string.Format("Email: {0}\r\n", response.EmailAddress);
            accountTextBox.Text += string.Format("First Name: {0}\r\n", response.FirstName);
            accountTextBox.Text += string.Format("Last Name: {0}\r\n", response.LastName);

            accountTextBox.Text += "Statistics: \r\n";
            accountTextBox.Text += string.Format("\tAccount Size: {0}\r\n", response.Statistics.AccountSize);
            accountTextBox.Text += string.Format("\tAvailable Account Size: {0}\r\n", response.Statistics.AvailableAccountSize);
            accountTextBox.Text += string.Format("\tDate Created: {0}\r\n", response.Statistics.DateCreated);
            accountTextBox.Text += string.Format("\tDate of Last Notice: {0}\r\n", response.Statistics.DateOfLastNotice);
            accountTextBox.Text += string.Format("\tDate of Last Visit: {0}\r\n", response.Statistics.DateOfLastVisit);
            accountTextBox.Text += string.Format("\tDate Password Expires: {0}\r\n", response.Statistics.DatePasswordExpires);
            accountTextBox.Text += string.Format("\tTotal Files In Outbox: {0}\r\n", response.Statistics.TotalFilesInOutbox);
            accountTextBox.Text += string.Format("\tTotal Files Sent: {0}", response.Statistics.TotalFilesSent);
            accountTextBox.Text += string.Format("\tTotal Messages In Inbox: {0}\r\n", response.Statistics.TotalMessagesInInbox);
            accountTextBox.Text += string.Format("\tTotal Messages In Outbox: {0}\r\n", response.Statistics.TotalMessagesInOutbox);
            accountTextBox.Text += string.Format("\tTotal Messages Received: {0}\r\n", response.Statistics.TotalMessagesReceived);
            accountTextBox.Text += string.Format("\tTotal Messages Sent: {0}\r\n", response.Statistics.TotalMessagesSent);
            accountTextBox.Text += string.Format("\tTotal Unread Messages In Inbox: {0}\r\n", response.Statistics.TotalUnreadMessagesInInbox);
            accountTextBox.Text += string.Format("\tTotal Visits: {0}\r\n", response.Statistics.TotalVisits);
            accountTextBox.Text += string.Format("\tUsed Account Size: {0}\r\n", response.Statistics.UsedAccountSize);

            EndWaitCursor();
        }

        // This button receives a response of type FolderResponses which is displayed through a for-loop
        private async void listFolderButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            listFoldersTextBox.Text = "";

            Folders.Folder response = new Folders.Folder();

            try
            {
                try
                {
                    response = await dmWeb.Folders.List();

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    listFoldersTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }

                int folderArrayLength = response.Folders.Count;
                for (int i = 0; i < folderArrayLength; i++)
                {
                    listFoldersTextBox.Text += string.Format("Folder Name: {0}\r\n", response.Folders[i].FolderName);
                    listFoldersTextBox.Text += string.Format("\tFolder Id: {0}\r\n", response.Folders[i].FolderId);
                    listFoldersTextBox.Text += string.Format("\tFolder Type: {0}\r\n", response.Folders[i].FolderType);
                    listFoldersTextBox.Text += string.Format("\tFolder Type Description: {0}\r\n", response.Folders[i].FolderTypeDescription);
                    listFoldersTextBox.Text += string.Format("\tIs System Folder: {0}\r\n", response.Folders[i].IsSystemFolder);
                    listFoldersTextBox.Text += string.Format("\tTotal Messages: {0}\r\n", response.Folders[i].TotalMessages);
                    listFoldersTextBox.Text += string.Format("\tTotal Size: {0}\r\n\r\n", response.Folders[i].TotalSize);
                }
            }
            catch
            {
                listFoldersTextBox.Text = "Session Key is missing";

                EndWaitCursor();
            }
        }

        // This button passes a string and an integer to create a folder in the user's account
        private async void createFolderButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            Folders.Create user = new Folders.Create();
            string response = "";
            int temp = 0;

            if (folderNameTextBox.Text == "" && folderTypeTextBox.Text == "")
            {
                listFoldersTextBox.Text = "Folder Name and Folder Type are missing";

                EndWaitCursor();
            }
            else
            {
                user.FolderName = folderNameTextBox.Text;
                if (folderTypeTextBox.Text == "")
                {
                    listFoldersTextBox.Text = "Folder Type is missing. Enter 0 or 1";

                    EndWaitCursor();
                }
                else
                {
                    if (int.TryParse(folderTypeTextBox.Text, out temp) == true)
                    {
                        user.FolderType = int.Parse(folderTypeTextBox.Text);

                        try
                        {
                            response = await dmWeb.Folders.Create(user);
                            folderNameTextBox.Text = "";
                            folderTypeTextBox.Text = "";
                            listFoldersTextBox.Text = string.Format("Newly Created Folder Id: {0}", response);

                            EndWaitCursor();
                        }
                        catch (Exception ex)
                        {
                            listFoldersTextBox.Text = ex.Message;

                            EndWaitCursor();
                            return;
                        }
                    }
                    else
                    {
                        listFoldersTextBox.Text = "Invalid folder type. Enter a 0 or 1.";

                        EndWaitCursor();
                    }
                }

                folderNameTextBox.Text = "";
                folderTypeTextBox.Text = "";
            }
        }

        // This button passes a FolderID to delete the selected folder
        private async void deleteFolder_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            string response = "";
            int temp;

            string FolderID = folderIDTextBox.Text;
            if (FolderID != "")
            {
                if (int.TryParse(FolderID, out temp) == true)
                {
                    try
                    {
                        response = await dmWeb.Folders.Delete(FolderID);
                        folderIDTextBox.Text = "";
                        listFoldersTextBox.Text = "Folder successfully deleted.";

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        listFoldersTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }
                }
                else
                {
                    listFoldersTextBox.Text = "Invalid FolderId entered.";

                    EndWaitCursor();
                }
            }
            else
            {
                listFoldersTextBox.Text = "FolderID field is empty";

                EndWaitCursor();
            }

            folderIDTextBox.Text = "";
        }

        // This button passes a FolderID to receive a response of type SummariesResponseBody which is displayed using a for-loop
        private async void getMessageSummariesButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            messageSummariesTextBox.Text = "";

            Messaging.GetMessageSummariesRequest user = new Messaging.GetMessageSummariesRequest();
            Messaging.GetMessageSummaries response = new Messaging.GetMessageSummaries();
            int temp = 0;

            if (folderIDTextBox2.Text == "")
            {
                messageSummariesTextBox.Text = "FolderID is missing";

                EndWaitCursor();
            }
            else
            {
                if (int.TryParse(folderIDTextBox2.Text, out temp) == false)
                {
                    messageSummariesTextBox.Text = "Invalid input (FID).";

                    EndWaitCursor();
                }
                else
                {
                    user.FolderId = int.Parse(folderIDTextBox2.Text);

                    if (lastMessageIdTextBox.Text != "")
                    {
                        if (int.TryParse(lastMessageIdTextBox.Text, out temp) == false)
                        {
                            messageSummariesTextBox.Text = "Invalid input (MID).";

                            EndWaitCursor();
                        }
                        else
                        {
                            user.LastMessageIDReceived = int.Parse(lastMessageIdTextBox.Text);

                            try
                            {
                                response = await dmWeb.Message.GetMessageSummaries(user);
                                folderIDTextBox2.Text = "";
                                lastMessageIdTextBox.Text = "";

                                EndWaitCursor();
                            }
                            catch (Exception ex)
                            {
                                messageSummariesTextBox.Text = ex.Message;

                                EndWaitCursor();
                                return;
                            }
                        }
                    }
                    else
                    {
                        user.LastMessageIDReceived = 0;

                        try
                        {
                            response = await dmWeb.Message.GetMessageSummaries(user);
                            folderIDTextBox2.Text = "";
                            lastMessageIdTextBox.Text = "";

                            EndWaitCursor();
                        }
                        catch (Exception ex)
                        {
                            messageSummariesTextBox.Text = ex.Message;

                            EndWaitCursor();
                            return;
                        }
                    }
                }

                int arrayLength = response.Summaries.Count;

                //messageSummariesTextBox.Text += string.Format("More Messages Available: {0}\r\n", response.MoreMessagesAvailable);

                for (int i = arrayLength - 1; i >= 0; i--)
                {
                    messageSummariesTextBox.Text += string.Format("Subject: {0}\r\n", response.Summaries[i].Subject);
                    messageSummariesTextBox.Text += string.Format("\tSender Address: {0}\r\n", response.Summaries[i].SenderAddress);
                    messageSummariesTextBox.Text += string.Format("\tMessage Id: {0}\r\n", response.Summaries[i].MessageId);
                    messageSummariesTextBox.Text += string.Format("\tFolder Id: {0}\r\n", response.Summaries[i].FolderId);
                    messageSummariesTextBox.Text += string.Format("\tCreate Time String: {0}\r\n", response.Summaries[i].createTimeString);
                    messageSummariesTextBox.Text += string.Format("\tRead: {0}\r\n\r\n", response.Summaries[i].Read);
                    //messageSummariesTextBox.Text += string.Format("\tAttachment Count: {0}\r\n", response.Summaries[i].Subject);
                    //messageSummariesTextBox.Text += string.Format("\tMessage Size: {0}\r\n", response.Summaries[i].Subject);
                    //messageSummariesTextBox.Text += string.Format("\tMessage Status: {0}\r\n", response.Summaries[i].Subject);
                }

                EndWaitCursor();
            }
        }

        // This button passes a MessageID to view the details about the specific message
        private async void newReadMessageSubmitButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            readMessageTextBox.Text = "";
            readMessageListBox.Items.Clear();
            dmWeb._base64.Clear();

            Messaging.GetMessage messageResponse = new Messaging.GetMessage();
            int temp = 0;


            if (messageIDTextBox.Text != "")
            {
                string messageId = messageIDTextBox.Text;
                if (int.TryParse(messageId, out temp) == true)
                {
                    try
                    {
                        messageResponse = await dmWeb.Message.Get(messageId);
                        messageIDTextBox.Text = "";

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        readMessageTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }

                    int toArrayLength = messageResponse.To.Count;
                    int ccArrayLength = messageResponse.Cc.Count;
                    int bccArrayLength = messageResponse.Bcc.Count;

                    readMessageTextBox.Text += "To: \r\n";
                    for (int i = 0; i < toArrayLength; i++)
                    {
                        readMessageTextBox.Text += string.Format("\t{0} \r\n", Regex.Match(messageResponse.To[i], @"\<([^>]*)\>").Groups[1].Value);
                    }

                    readMessageTextBox.Text += "Cc: \r\n";
                    for (int i = 0; i < ccArrayLength; i++)
                    {
                        readMessageTextBox.Text += string.Format("\t{0} \r\n", Regex.Match(messageResponse.Cc[i], @"\<([^>]*)\>").Groups[1].Value);
                    }

                    readMessageTextBox.Text += "Bcc: \r\n";
                    for (int i = 0; i < bccArrayLength; i++)
                    {
                        readMessageTextBox.Text += string.Format("\t{0} \r\n", Regex.Match(messageResponse.Bcc[i], @"\<([^>]*)\>").Groups[1].Value);
                    }

                    readMessageTextBox.Text += string.Format("Subject: {0}\r\n", messageResponse.Subject);
                    readMessageTextBox.Text += string.Format("Create Time: {0}\r\n", messageResponse.CreateTime);

                    int attachmentArrayLength = messageResponse.Attachments.Count;
                    for (int i = 0; i < attachmentArrayLength; i++)
                    {
                        //readMessageTextBox.Text += string.Format("\t{0}\r\n", messageResponse.Attachments[i].AttachmentBase64);
                        //readMessageTextBox.Text += string.Format("\t{0}\r\n", messageResponse.Attachments[i].ContentType);
                        //readMessageTextBox.Text += string.Format("\t{0}\r\n", messageResponse.Attachments[i].FileName);
                        readMessageListBox.Items.Add(messageResponse.Attachments[i].FileName);
                        dmWeb._base64.Add(messageResponse.Attachments[i].AttachmentBase64);
                    }

                    //readMessageTextBox.Text += string.Format("\t{0}\r\n", messageResponse.HtmlBody);
                    readMessageTextBox.Text += string.Format("Text Body: \r\n\t{0}", messageResponse.TextBody);
                }
                else
                {
                    readMessageTextBox.Text = "Invalid input (MID).";

                    EndWaitCursor();
                }
            }
            else
            {
                readMessageTextBox.Text = "Enter a MessageID";

                EndWaitCursor();
            }

            EndWaitCursor();
        }

        // This button passes the old password and new password to change the account's password
        private async void passwordChangeButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            Account.ChangePassword user = new Account.ChangePassword();
            string response = "";

            user.OldPassword = oldpasswordTextBox.Text;
            user.NewPassword = newpasswordTextBox.Text;

            if (user.OldPassword != "" && user.NewPassword != "")
            {
                try
                {
                    response = await dmWeb.Account.ChangePassword(user);
                    accountTextBox.Text = response;

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    accountTextBox.Text = ex.Message;

                    EndWaitCursor();
                }

                oldpasswordTextBox.Text = "";
                newpasswordTextBox.Text = "";
            }
            else
            {
                accountTextBox.Text = "Old Password and/or New Password missing";

                EndWaitCursor();
            }
        }

        // This button passes a MessageID and a FolderID to move the selected message to the selected folder
        private async void moveButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            Messaging.MoveMessageRequest user = new Messaging.MoveMessageRequest();
            int temp = 0;
            string response = "";

            if (messageIDTextBox2.Text != "" || destinationFolderIDTextBox.Text != "")
            {
                if (int.TryParse(messageIDTextBox2.Text, out temp) == true)
                {
                    if (int.TryParse(destinationFolderIDTextBox.Text, out temp) == true)
                    {
                        user.DestinationFolderId = int.Parse(destinationFolderIDTextBox.Text);
                        int MessageId = int.Parse(messageIDTextBox2.Text);

                        try
                        {
                            response = await dmWeb.Message.Move(user, MessageId.ToString());
                            messageIDTextBox2.Text = "";
                            destinationFolderIDTextBox.Text = "";
                            messageOperationsTextBox.Text = "Message moved";

                            EndWaitCursor();
                        }
                        catch (Exception ex)
                        {
                            messageOperationsTextBox.Text = ex.Message;

                            EndWaitCursor();
                            return;
                        }
                    }
                    else
                    {
                        messageOperationsTextBox.Text = "Invalid input (Destination FID).";

                        EndWaitCursor();
                    }
                }
                else
                {
                    messageOperationsTextBox.Text = "Invalid input (MID).";

                    EndWaitCursor();
                }
            }
            else
            {
                messageOperationsTextBox.Text = "Enter a MessageID and DestinationFolderID";

                EndWaitCursor();
            }
        }

        // This button passes a MessageID and deletes the message to the trash or permanently
        private async void deleteButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            bool permanentCheck;
            int temp = 0;

            if (permDeleteCheckBox.Checked)
            {
                permanentCheck = true;
            }
            else
            {
                permanentCheck = false;
            }
            Messaging.DeleteMessageResponse user = new Messaging.DeleteMessageResponse();
            if (messageIDTextBox3.Text != "")
            {
                if (int.TryParse(messageIDTextBox3.Text, out temp) == true)
                {
                    int MessageId = int.Parse(messageIDTextBox3.Text);
                    try
                    {
                        await dmWeb.Message.Delete(MessageId.ToString(), permanentCheck);
                        messageIDTextBox3.Text = "";

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        messageOperationsTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }

                    messageOperationsTextBox.Text = "Message deleted";

                    EndWaitCursor();
                }
                else
                {
                    messageOperationsTextBox.Text = "Invalid input (MID).";

                    EndWaitCursor();
                }
            }
            else
            {
                messageOperationsTextBox.Text = "Enter a MessageID to delete";

                EndWaitCursor();
            }
        }

        // This button passes a MessageID to be retracted
        private async void retractButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            int temp = 0;

            if (messageIDTextBox4.Text != "")
            {
                if (int.TryParse(messageIDTextBox4.Text, out temp))
                {
                    int MessageId = int.Parse(messageIDTextBox4.Text);
                    try
                    {
                        string response = await dmWeb.Message.Retract(MessageId.ToString());
                        messageIDTextBox4.Text = "";
                        messageOperationsTextBox.Text = "Message retracted";

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        messageOperationsTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }
                }
                else
                {
                    messageOperationsTextBox.Text = "Invalid input (MID).";

                    EndWaitCursor();
                }
            }
            else
            {
                messageOperationsTextBox.Text = "Enter a MessageID to retract";

                EndWaitCursor();
            }
        }

        // This button passes the information needed to send a message
        private async void sendButton_Click(object sender, EventArgs e)
        {
            sendMessageTextBox.Text = "";

            StartWaitCursor();

            if (toTextBox.Text != "")
            {
                string sendToString = toTextBox.Text;
                string[] sendToStringArray = sendToString.Split(',');
                foreach (string str in sendToStringArray)
                {
                    dmWeb.sendMessagePayload.To.Add(str);
                }
            }
            else
            {
                sendMessageTextBox.Text = "Enter an email to send to.";

                EndWaitCursor();
                return;
            }

            string ccString = ccTextBox.Text;
            string[] ccStringArray = ccString.Split(',');
            foreach (string str in ccStringArray)
            {
                dmWeb.sendMessagePayload.Cc.Add(str);
            }

            string bccString = bccTextBox.Text;
            string[] bccStringArray = bccString.Split(',');
            foreach (string str in bccStringArray)
            {
                dmWeb.sendMessagePayload.Bcc.Add(str);
            }
            dmWeb.sendMessagePayload.Subject = subjectTextBox.Text;
            dmWeb.sendMessagePayload.TextBody = messageRichTextBox.Text;

            try
            {
                int mid = await dmWeb.Message.Send(dmWeb.sendMessagePayload);
                sendMessageTextBox.Text = string.Format("MessageId: {0}", mid);

                toTextBox.Text = "";
                ccTextBox.Text = "";
                bccTextBox.Text = "";
                subjectTextBox.Text = "";
                messageRichTextBox.Text = "";
                sendMessageListBox.Items.Clear();
                dmWeb.sendMessagePayload.Attachments.Clear();

                EndWaitCursor();
            }
            catch (Exception ex)
            {
                sendMessageTextBox.Text = ex.Message;

                EndWaitCursor();
                return;
            }
        }

        // This button allows the user to add an attachment and converts it to a Base64 string
        private void attachmentButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.ShowDialog();

            foreach (string file in dlg.FileNames)
            {
                System.IO.FileInfo location = new System.IO.FileInfo(file);
                string base64String = dmWeb.ConvertToBase64(location.ToString());

                dmWeb.sendMessagePayload.Attachments.Add(new Messaging.AttachmentsBody() { AttachmentBase64 = base64String, ContentType = Path.GetExtension(file), FileName = Path.GetFileName(file) });

                sendMessageListBox.Items.Add(Path.GetFileNameWithoutExtension(file));
            }

            EndWaitCursor();
        }

        // This listbox allows the user to click an attachment and save it to their desired destination
        private void readMessageListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartWaitCursor();

            if (readMessageListBox.SelectedItem != null)
            {
                string fileName = readMessageListBox.GetItemText(readMessageListBox.SelectedItem);
                string fileType = Path.GetExtension(fileName);
                fileName += fileType;

                dmWeb.ConvertFromBase64(dmWeb._base64[readMessageListBox.SelectedIndex], fileName);
            }

            EndWaitCursor();
        }

        // This button allows the user to remove an attachment from a message
        private void removeAttachmentButton_Click(object sender, EventArgs e)
        {
            StartWaitCursor();

            if (sendMessageListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an attachment");

                EndWaitCursor();
            }
            else
            {
                dmWeb.sendMessagePayload.Attachments.RemoveAt(sendMessageListBox.SelectedIndex);
                sendMessageListBox.Items.RemoveAt(sendMessageListBox.SelectedIndex);

                EndWaitCursor();
            }
        }

        // This button toggles the password visibility of LogOn
        private void passwordVisibilityTextBox_CheckedChanged(object sender, EventArgs e)
        {
            if (passwordVisibilityTextBox.Checked)
            {
                passwordTextBox.PasswordChar = '\0';
            }
            else
            {
                passwordTextBox.PasswordChar = '*';
            }
        }

        // This button toggles the password visibility of ChangePassword
        private void passwordVisibilityCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (passwordVisibilityCheckBox2.Checked)
            {
                oldpasswordTextBox.PasswordChar = '\0';
                newpasswordTextBox.PasswordChar = '\0';
            }
            else
            {
                oldpasswordTextBox.PasswordChar = '*';
                newpasswordTextBox.PasswordChar = '*';
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            toTextBox.Text = "";
            ccTextBox.Text = "";
            bccTextBox.Text = "";
            subjectTextBox.Text = "";
            messageRichTextBox.Text = "";
            sendMessageTextBox.Text = "Text fields cleared.";
        }

        private async void showDelegatesButton_Click(object sender, EventArgs e)
        {
            groupBoxRichTextBox.Text = "";

            StartWaitCursor();

            Group_Mailbox.ShowDelegatesResponse response = new Group_Mailbox.ShowDelegatesResponse();

            try
            {
                response = await dmWeb.GroupInbox.ShowDelegates();

                EndWaitCursor();
            }
            catch (Exception ex)
            {
                groupBoxRichTextBox.Text = ex.Message;

                EndWaitCursor();
                return;
            }

            groupBoxRichTextBox.Text = "Delegates: \r\n";
            int delegateArrayLength = response.Delegates.Length;
            for (int i = 0; i < delegateArrayLength; i++)
            {
                groupBoxRichTextBox.Text += string.Format("\tDelegateAddress: {0} \r\n", response.Delegates[i].DelegateAddress);
                groupBoxRichTextBox.Text += string.Format("\tCreated: {0} \r\n\r\n", response.Delegates[i].Created);
            }
        }

        private async void deleteDelegateButton_Click(object sender, EventArgs e)
        {
            string response = "";

            StartWaitCursor();

            if (delegateAddressTextBox.Text != "")
            {
                string delegateAddress = delegateAddressTextBox.Text;

                try
                {
                    response = await dmWeb.GroupInbox.DeleteDelegate(delegateAddress);
                    groupBoxRichTextBox.Text = "Delegated deleted";
                    delegateAddressTextBox.Text = "";

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }
            else
            {
                groupBoxRichTextBox.Text = "Enter a delegate address";
            }
        }

        private async void addDelegateButton_Click(object sender, EventArgs e)
        {
            Group_Mailbox.AddDelegateRequest request = new Group_Mailbox.AddDelegateRequest();
            string response = "";

            StartWaitCursor();

            if (delegateAddressTextBox.Text != "")
            {
                request.DelegateAddress = delegateAddressTextBox.Text;

                try
                {
                    response = await dmWeb.GroupInbox.AddDelegate(request);
                    groupBoxRichTextBox.Text = "Delegated added";
                    delegateAddressTextBox.Text = "";

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }
            else
            {
                groupBoxRichTextBox.Text = "Enter a delegate address";
            }
        }

        private async void showGroupBoxButton_Click(object sender, EventArgs e)
        {
            groupBoxRichTextBox.Text = "";

            Group_Mailbox.ShowGroupBoxesResponse response = new Group_Mailbox.ShowGroupBoxesResponse();

            StartWaitCursor();

            try
            {
                response = await dmWeb.GroupInbox.ShowGroupBoxes();

                EndWaitCursor();
            }
            catch (Exception ex)
            {
                groupBoxRichTextBox.Text = ex.Message;

                EndWaitCursor();
                return;
            }

            groupBoxRichTextBox.Text += "GroupBoxes: \r\n";
            int groupBoxArrayLength = response.GroupBox.Length;
            for (int i = 0; i < groupBoxArrayLength; i++)
            {
                groupBoxRichTextBox.Text += string.Format("\tGroupBoxAddress: {0}\r\n", response.GroupBox[i].GroupBoxAddress);
                groupBoxRichTextBox.Text += string.Format("\tCreated: {0}\r\n\r\n", response.GroupBox[i].Created);
            }
        }

        private async void getGroupInboxMIDButton_Click(object sender, EventArgs e)
        {
            groupBoxRichTextBox.Text = "";

            Group_Mailbox.GetGroupInboxMIDsRequest request = new Group_Mailbox.GetGroupInboxMIDsRequest();
            Group_Mailbox.GetGroupInboxMIDsResponse response = new Group_Mailbox.GetGroupInboxMIDsResponse();

            StartWaitCursor();

            if (mustHaveAttachmentsCheckBox.Checked == true)
            {
                request.MustHaveAttachments = true;

                try
                {
                    response = await dmWeb.GroupInbox.GetGroupInboxMessageIds(request);

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }
            else
            {
                request.MustHaveAttachments = false;

                try
                {
                    response = await dmWeb.GroupInbox.GetGroupInboxMessageIds(request);

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }

            groupBoxRichTextBox.Text += "Group Inbox MessageIds \r\n";
            int midArrayLength = response.MessageIds.Length;
            for (int i = 0; i < midArrayLength; i++)
            {
                groupBoxRichTextBox.Text += string.Format("\tMessageId: {0}\r\n", response.MessageIds[i]);
            }
        }

        private async void getGroupMessageSummaries_Click(object sender, EventArgs e)
        {
            groupBoxRichTextBox.Text = "";

            Group_Mailbox.GetGroupMessageSummariesRequest request = new Group_Mailbox.GetGroupMessageSummariesRequest();
            Group_Mailbox.GetGroupMessageSummariesResponse response = new Group_Mailbox.GetGroupMessageSummariesResponse();
            int temp = 0;

            StartWaitCursor();

            if (lastMessageIdReceivedTextBox.Text != "")
            {
                if (int.TryParse(lastMessageIdReceivedTextBox.Text, out temp) != false)
                {
                    request.LastMessageIdReceived = int.Parse(lastMessageIdReceivedTextBox.Text);

                    try
                    {
                        response = await dmWeb.GroupInbox.GetGroupMessageSummaries(request);

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        groupBoxRichTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }
                }
                else
                {
                    groupBoxRichTextBox.Text = "Enter a valid Message Id";
                }
            }
            else
            {
                request.LastMessageIdReceived = 0;

                try
                {
                    response = await dmWeb.GroupInbox.GetGroupMessageSummaries(request);

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }

            groupBoxRichTextBox.Text = "Group Message Summaries: \r\n";
            groupBoxRichTextBox.Text += string.Format("\tMoreMessagesAvailable: {0}\r\n", response.MoreMessagesAvailable);
            groupBoxRichTextBox.Text += string.Format("\tSummaries: \r\n");

            int summaryArrayLength = response.Summaries.Length;
            for (int i = 0; i < summaryArrayLength; i++)
            {
                groupBoxRichTextBox.Text += string.Format("\t\tMessageId: {0} \r\n", response.Summaries[i].MessageId);
                groupBoxRichTextBox.Text += string.Format("\t\tSubject: {0} \r\n", response.Summaries[i].Subject);
                groupBoxRichTextBox.Text += string.Format("\t\tCreated: {0} \r\n", response.Summaries[i].Created);
                groupBoxRichTextBox.Text += string.Format("\t\tFromAddress: {0} \r\n", response.Summaries[i].FromAddress);
                groupBoxRichTextBox.Text += string.Format("\t\tToAddress: {0} \r\n", response.Summaries[i].ToAddress);
                groupBoxRichTextBox.Text += string.Format("\t\tSize: {0} \r\n", response.Summaries[i].Size);
                groupBoxRichTextBox.Text += string.Format("\t\tMessageStatus: {0} \r\n\r\n", response.Summaries[i].MessageStatus);

            }
        }

        private async void getGroupInbox_Click(object sender, EventArgs e)
        {
            Group_Mailbox.GroupInboxResponse response = new Group_Mailbox.GroupInboxResponse();
            int temp = 0;

            StartWaitCursor();

            if (lastMessageIdReceivedTextBox.Text != "")
            {
                if (int.TryParse(lastMessageIdReceivedTextBox.Text, out temp) != false)
                {
                    int mid = int.Parse(lastMessageIdReceivedTextBox.Text);

                    try
                    {
                        response = await dmWeb.GroupInbox.GroupInbox(mid.ToString());

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        groupBoxRichTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }
                }
                else
                {
                    groupBoxRichTextBox.Text = "Enter a Message Id";
                }
            }
            else
            {
                try
                {
                    response = await dmWeb.GroupInbox.GroupInbox("");

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }

            groupBoxRichTextBox.Text = "Group Inbox (Read): \r\n";
            groupBoxRichTextBox.Text += string.Format("\tMoreMessagesAvailable: {0}\r\n", response.MoreMessagesAvailable);
            groupBoxRichTextBox.Text += string.Format("\tSummaries: \r\n");

            int summaryArrayLength = response.Summaries.Length;
            for (int i = 0; i < summaryArrayLength; i++)
            {
                groupBoxRichTextBox.Text += string.Format("\t\tMessageId: {0} \r\n", response.Summaries[i].MessageId);
                groupBoxRichTextBox.Text += string.Format("\t\tSubject: {0} \r\n", response.Summaries[i].Subject);
                groupBoxRichTextBox.Text += string.Format("\t\tCreated: {0} \r\n", response.Summaries[i].Created);
                groupBoxRichTextBox.Text += string.Format("\t\tFromAddress: {0} \r\n", response.Summaries[i].FromAddress);
                groupBoxRichTextBox.Text += string.Format("\t\tToAddress: {0} \r\n", response.Summaries[i].ToAddress);
                groupBoxRichTextBox.Text += string.Format("\t\tSize: {0} \r\n", response.Summaries[i].Size);
                groupBoxRichTextBox.Text += string.Format("\t\tMessageStatus: {0} \r\n\r\n", response.Summaries[i].MessageStatus);

            }
        }

        private async void getGroupInboxUnread_Click(object sender, EventArgs e)
        {
            Group_Mailbox.GetGroupInboxUnreadResponse response = new Group_Mailbox.GetGroupInboxUnreadResponse();
            int temp = 0;

            StartWaitCursor();

            if (lastMessageIdReceivedTextBox.Text != "")
            {
                if (int.TryParse(lastMessageIdReceivedTextBox.Text, out temp) != false)
                {
                    int mid = int.Parse(lastMessageIdReceivedTextBox.Text);

                    try
                    {
                        response = await dmWeb.GroupInbox.GetGroupInboxUnread(mid.ToString());

                        EndWaitCursor();
                    }
                    catch (Exception ex)
                    {
                        groupBoxRichTextBox.Text = ex.Message;

                        EndWaitCursor();
                        return;
                    }
                }
                else
                {
                    groupBoxRichTextBox.Text = "Enter a Message Id";
                }
            }
            else
            {
                try
                {
                    response = await dmWeb.GroupInbox.GetGroupInboxUnread("");

                    EndWaitCursor();
                }
                catch (Exception ex)
                {
                    groupBoxRichTextBox.Text = ex.Message;

                    EndWaitCursor();
                    return;
                }
            }

            groupBoxRichTextBox.Text = "Group Inbox (Unread): \r\n";
            groupBoxRichTextBox.Text += string.Format("\tMoreMessagesAvailable: {0}\r\n", response.MoreMessagesAvailable);
            groupBoxRichTextBox.Text += string.Format("\tSummaries: \r\n");

            int summaryArrayLength = response.Summaries.Length;
            for (int i = 0; i < summaryArrayLength; i++)
            {
                groupBoxRichTextBox.Text += string.Format("\t\tMessageId: {0} \r\n", response.Summaries[i].MessageId);
                groupBoxRichTextBox.Text += string.Format("\t\tSubject: {0} \r\n", response.Summaries[i].Subject);
                groupBoxRichTextBox.Text += string.Format("\t\tCreated: {0} \r\n", response.Summaries[i].Created);
                groupBoxRichTextBox.Text += string.Format("\t\tFromAddress: {0} \r\n", response.Summaries[i].FromAddress);
                groupBoxRichTextBox.Text += string.Format("\t\tToAddress: {0} \r\n", response.Summaries[i].ToAddress);
                groupBoxRichTextBox.Text += string.Format("\t\tSize: {0} \r\n", response.Summaries[i].Size);
                groupBoxRichTextBox.Text += string.Format("\t\tMessageStatus: {0} \r\n\r\n", response.Summaries[i].MessageStatus);

            }
        }
    }  
}
