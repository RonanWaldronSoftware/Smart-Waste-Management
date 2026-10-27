namespace SmartWaste.Model.Helpers.Email
{
    public class EmailAttachmentModel
    {
        public string Content { get; set; }
        public string Type { get; set; }
        public string Disposition { get; set; } = "attachment";
        public string FileName { get; set; }
    }

    public class EmailSendModel
    {
        public EmailFromModel From { get; set; }
        public List<EmailToModel> To { get; set; } = new();
        public List<EmailAttachmentModel> Attachments { get; set; } = new();
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public string Html { get; set; }
    }

    public class EmailFromModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class EmailToModel
    {
        public string Email { get; set; }
    }
}
