using System.Xml.Serialization;
using System.Collections.Generic;

namespace MobileMilk.Data.Messages
{
    [XmlRoot("rsp")]
    public class RtmGetTasksResponse
    {
        [XmlAttribute("stat")]
        public string Status { get; set; }

        [XmlElement("tasks")]
        public RtmTasksResponse Tasks { get; set; }
    }

    public class RtmTasksResponse
    {
        [XmlElement("list")]
        public List<RtmTaskListResponse> List { get; set; }
    }

    public class RtmTaskListResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("taskseries")]
        public List<RtmTaskSeriesResponse> TaskSeries { get; set; }
    }

    public class RtmTaskSeriesResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("created")]
        public string Created { get; set; }
        [XmlAttribute("modified")]
        public string Modified { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("source")]
        public string Source { get; set; }
        [XmlAttribute("url")]
        public string Url { get; set; }
        [XmlAttribute("location_id")]
        public string LocationId { get; set; }

        [XmlArray("tags")]
        [XmlArrayItem("tag", typeof(string))]
        public List<string> Tags { get; set; }
        [XmlArray("participants")]
        [XmlArrayItem("contact", typeof(RtmContactResponse))]
        public List<RtmContactResponse> Participants { get; set; }
        [XmlArray("notes")]
        [XmlArrayItem("note", typeof(RtmNoteResponse))]
        public List<RtmNoteResponse> Notes { get; set; }

        [XmlElement("task")]
        public List<RtmTaskResponse> Tasks { get; set; }
    }

    public class RtmContactResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("fullname")]
        public string FullName { get; set; }
        [XmlAttribute("username")]
        public string UserName { get; set; }
    }

    public class RtmNoteResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("created")]
        public string Created { get; set; }
        [XmlAttribute("modified")]
        public string Modified { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }

        //TODO: figure out how to deserialize this value
        //[XmlArrayItem("note", typeof(string))]
        public string Text { get; set; }
    }

    public class RtmTaskResponse
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlAttribute("due")]
        public string Due { get; set; }
        [XmlAttribute("has_due_time")]
        public string HasDueTime { get; set; }
        [XmlAttribute("added")]
        public string Added { get; set; }
        [XmlAttribute("completed")]
        public string Completed { get; set; }
        [XmlAttribute("deleted")]
        public string Deleted { get; set; }
        [XmlAttribute("priority")]
        public string Priority { get; set; }
        [XmlAttribute("postponed")]
        public string Postponed { get; set; }
        [XmlAttribute("estimate")]
        public string Estimate { get; set; }
    }
}
