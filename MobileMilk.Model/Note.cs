using System;

namespace MobileMilk.Model
{
    public class Note
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}