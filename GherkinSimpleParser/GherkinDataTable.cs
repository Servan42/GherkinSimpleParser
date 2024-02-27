

namespace GherkinSimpleParser
{
    /// <summary>
    /// Represents a Datatable, usually written after an Instruction line.
    /// </summary>
    public class GherkinDataTable : List<List<string>>
    {
        internal GherkinDataTable DeepClone()
        {
            var clone = new GherkinDataTable();
            foreach (var row in this)
            {
                clone.Add(new List<string>(row));
            }
            return clone;
        }

        internal GherkinDataTable ResolveExampleValueInNewObject(string key, string value)
        {
            var newDatatable = new GherkinDataTable();
            foreach (var row in this)
            {
                newDatatable.Add(row.Select(item => item.Replace($"<{key}>", value)).ToList());
            }
            return newDatatable;
        }
    }
}