using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanelCore.DataTableAjax
{
    public class DataTableParameters
    {
        public List<DataTableColumn> Columns { get; set; }
        public int Draw { get; set; }
        public int Length { get; set; }
        public List<DataOrder> Order { get; set; }
        public Search Search { get; set; }
        public int Start { get; set; }
    }

    public class DataTableBootstrap
    {
        int draw { get; set; }
        int row { get; set; }
        int rowperpage { get; set; }
        int columnIndex { get; set; }
        string columnName { get; set; }
        string columnSortOrder { get; set; }
        string searchValue { get; set; }
        public DataTableBootstrap(DataTableParameters data)
        {
            draw = data.Draw;
            row = data.Start;
            rowperpage = data.Length;
            columnIndex = data.Order[0].Column;
            columnName = data.Columns[columnIndex].Data;
            columnSortOrder = data.Order[0].Dir;
            searchValue = data.Search.Value;
        }
    }

    public class Search
    {
        public string Value { get; set; }
    }

    public class DataTableColumn
    {
        public string Data { get; set; }
    }

    public class DataOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }
}
