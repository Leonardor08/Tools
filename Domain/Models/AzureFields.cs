using System.Text.Json.Serialization;

namespace Domain.Models;

public class AzureFields
{
    [JsonPropertyName("System.WorkItemType")]
    public string WorkItemType { get; set; } = string.Empty;

    [JsonPropertyName("System.State")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("System.Reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("System.AssignedTo")]
    public AssignedToProperties AssignedTo { get; set; } = new();

    [JsonPropertyName("System.CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("System.CreatedBy")]
    public AssignedToProperties CreatedBy { get; set; } = new();

    [JsonPropertyName("System.ChangedDate")]
    public DateTime ChangedDate { get; set; }

    [JsonPropertyName("System.ChangedBy")]
    public AssignedToProperties ChangedBy { get; set; } = new();

    [JsonPropertyName("System.CommentCount")]
    public int CommentCount { get; set; }

    [JsonPropertyName("System.TeamProject")]
    public string TeamProject { get; set; } = string.Empty;

    [JsonPropertyName("System.AreaPath")]
    public string AreaPath { get; set; } = string.Empty;

    [JsonPropertyName("System.IterationPath")]
    public string IterationPath { get; set; } = string.Empty;

    [JsonPropertyName("System.Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.VSTS.Common.Priority")]
    public int Priority { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.ValueArea")]
    public string ValueArea { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.VSTS.Common.Risk")]
    public string Risk { get; set; } = string.Empty;

    [JsonPropertyName("WEF_EF52B5C8B32A402093EA71CB9A84BD17_Kanban.Column.Done")]
    public bool ColumDone { get; set; }

    [JsonPropertyName("System.Description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("System.History")]
    public string History { get; set; } = string.Empty;

    [JsonPropertyName("System.BoardColumnDone")]
    public bool BoardColumnDone { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Scheduling.StoryPoints")]
    public double StoryPoints { get; set; }

    [JsonPropertyName("System.BoardColumn")]
    public string BoardColumn { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.VSTS.Common.StateChangeDate")]
    public DateTime StateChangeDate { get; set; }

    [JsonPropertyName("WEF_EF52B5C8B32A402093EA71CB9A84BD17_Kanban.Column")]
    public string KanbanColumn { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.VSTS.Common.StackRank")]
    public double StackRank { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.AcceptanceCriteria")]
    public string AcceptanceCriteria { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.VSTS.Scheduling.DueDate")]
    public DateTime DueDate { get; set; }
    [JsonPropertyName("System.Tags")]
    public string Tags { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.VSTS.Common.ActivatedBy")]
    public AssignedToProperties ActivatedBy { get; set; } = new();

    [JsonPropertyName("Microsoft.VSTS.Common.ActivatedDate")]
    public DateTime? ActivatedDate { get; set; }
}