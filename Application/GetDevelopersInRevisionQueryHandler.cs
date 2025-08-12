using Application.DTO;
using Application.Interfaces;
using Domain.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;

namespace Application
{
    public class GetDevelopersInRevisionQueryHandler(IRevisionRepository revisionRepository)
    {
        private readonly IRevisionRepository _revisionRepository = revisionRepository;

        public async Task<List<AssignedDetailDTO>> Handle(string organization, string project, string workItemId, CancellationToken cancellationToken)
        {
            List<AssignedDetailDTO> tempList = [];

            //List<string> developers = ["Fernando Bohorquez Ascanio", "Andres Felipe Naranjo Grisales", "Jhoan Sebastian Villalba Morales"];
            Revision revisions = await _revisionRepository.GetRevisionsByWorkItem(organization, project, workItemId, cancellationToken);
            var workItems = revisions.Value;
            string fechaFinal = string.Empty;
            string totalAssigmentTime = string.Empty;

            for (int i = 0; i < revisions.Count; i++)
            {
                RevisionProperties current = workItems[i];

                if (i + 1 < workItems.Count)
                {
                    fechaFinal = workItems[i + 1].Fields.ChangedDate.ToString("dd/MM/yyyy HH:mm");
                    DateTime finisDate = workItems[i + 1].Fields.ChangedDate;
                    DateTime actualDate = current.Fields.ChangedDate;
                    TimeSpan timeDifference = finisDate.Subtract(actualDate);
                    int totalHours = (int)timeDifference.TotalHours;
                }

                List<string> comments = workItems
                    .Where(r =>
                        r.Fields.ChangedBy.DisplayName == current.Fields.AssignedTo.DisplayName &&
                        r.Fields.ChangedDate >= current.Fields.ChangedDate &&
                        r.Fields.ChangedDate < DateTime.MaxValue &&
                        !string.IsNullOrWhiteSpace(r.Fields.History))
                    .Select(r => r.Fields.History.Trim())
                    .ToList();

                tempList.Add(new AssignedDetailDTO()
                {
                    AssignedTo = current.Fields.AssignedTo.DisplayName,
                    ChangedBy = current.Fields.ChangedBy.DisplayName,
                    History = comments.Count != 0 ? string.Join(" | ", comments.Select(comment => IsHtml(comment) ? CleanHtml(comment) : comment)) : "",
                    AssignedDate = current.Fields.ChangedDate.ToString("dd/MM/yyyy HH:mm"),
                    EndDate = fechaFinal,
                    DueDate = current.Fields.DueDate.ToString("dd/MM/yyyy HH:mm"),
                });
            }

            return tempList;
        }

        public static string CleanHtml(string html)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(html);

            string plainText = doc.DocumentNode.InnerText;
            plainText = HttpUtility.HtmlDecode(plainText);

            return plainText;
        }

        private static bool IsHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            return input.StartsWith("<") && input.EndsWith(">")
                || input.Contains("</") 
                || Regex.IsMatch(input, "<.*?>");
        }
    }
}
