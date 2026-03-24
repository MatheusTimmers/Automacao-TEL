using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using System.Data.SqlTypes;
using System.Drawing;

namespace DataBaseClass
{
    public class DataBaseCommands
    {
        private static readonly HashSet<string> ValidTableNames = new HashSet<string>
        {
            "OccupiedBandwidth6dB",
            "OccupiedBandwidth26dB",
            "OccupiedBandwidth20dB",
            "MaximumPeakPower",
            "AverageMaximumOutputPower",
            "PeakPowerSpectralDensity",
            "AveragePowerSpectralDensity",
            "OutOfBandEmissions",
            "OutputPower",
            "PowerSpectralDensity",
            "HoppingChannelSeparation",
            "NumberOfOccupations",
            "OccupationTime",
        };

        private FbConnection CreateConnection()
        {
            var dir = Directory.GetCurrentDirectory();
            if (dir == null)
                return null;
            dir = Path.Combine(dir, "entrypoint\\BancoDeDados\\AUTOMACAOTEL.fdb");
            return new FbConnection($"User=SYSDBA;Password=pafuncio;Database={dir};DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;");
        }

        private FbParameterCollection sqlParameterCollection = new FbCommand().Parameters;

        public void ClearParameters()
        {
            sqlParameterCollection.Clear();
        }

        public void AddParameters(string name, object value)
        {
            sqlParameterCollection.Add(new FbParameter(name, value));
        }

        public void AddColumns(ref DataSet sqlTable, string nameColumn, Type typeColumn)
        {
            DataTable RequestTable = sqlTable.Tables.Add("Request");
            RequestTable.Columns.Add(nameColumn, typeColumn);
        }

        public void AddColumns(ref DataSet sqlTable, string nameColumn, Type typeColumn, bool pkeyColumn)
        {
            DataTable RequestTable = sqlTable.Tables.Add("Request");
            DataColumn pkOrderID = RequestTable.Columns.Add(nameColumn, typeColumn);
            RequestTable.PrimaryKey = new DataColumn[] { pkOrderID };
        }

        public void ExecuteCommand(CommandType commandType, string nameStoredProcedureOrTextSql)
        {
            using (FbConnection sqlConnection = CreateConnection())
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                FbTransaction orderTrans = sqlConnection.BeginTransaction();
                FbCommand cmd = sqlConnection.CreateCommand();
                cmd.Transaction = orderTrans;

                try
                {
                    cmd.CommandType    = commandType;
                    cmd.CommandText    = nameStoredProcedureOrTextSql;
                    cmd.CommandTimeout = 500;

                    foreach (FbParameter sqlParameter in sqlParameterCollection)
                        cmd.Parameters.Add(new FbParameter(sqlParameter.ParameterName, sqlParameter.Value));

                    cmd.ExecuteNonQuery();
                    orderTrans.Commit();
                }
                catch (Exception e)
                {
                    orderTrans.Rollback();
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    orderTrans.Dispose();
                    sqlConnection.Close();
                }
            }
        }

        public MemoryStream ExecuteQuery(CommandType commandType, string nameStoredProcedureOrTextSql)
        {
            using (FbConnection sqlConnection = CreateConnection())
            {
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();

                FbTransaction orderTrans = sqlConnection.BeginTransaction();
                FbCommand cmd = sqlConnection.CreateCommand();
                cmd.Transaction = orderTrans;

                try
                {
                    cmd.CommandType    = commandType;
                    cmd.CommandText    = nameStoredProcedureOrTextSql;
                    cmd.CommandTimeout = 500;

                    foreach (FbParameter sqlParameter in sqlParameterCollection)
                        cmd.Parameters.Add(new FbParameter(sqlParameter.ParameterName, sqlParameter.Value));

                    FbDataAdapter da = new FbDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    orderTrans.Commit();

                    return new MemoryStream((byte[])ds.Tables[0].Rows[0]["USERIMAGE"]);
                }
                catch (Exception e)
                {
                    orderTrans.Rollback();
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    orderTrans.Dispose();
                    sqlConnection.Close();
                }
            }
        }

        public DataTable GetTable(FbDataReader reader)
        {
            DataTable tbSchema = reader.GetSchemaTable();
            DataTable tbReturn = new DataTable();

            foreach (DataRow r in tbSchema.Rows)
            {
                if (!tbReturn.Columns.Contains(r["ColumnName"].ToString()))
                {
                    DataColumn col = new DataColumn()
                    {
                        ColumnName  = r["ColumnName"].ToString(),
                        Unique      = Convert.ToBoolean(r["IsUnique"]),
                        AllowDBNull = Convert.ToBoolean(r["AllowDBNull"]),
                        ReadOnly    = Convert.ToBoolean(r["IsReadOnly"])
                    };
                    tbReturn.Columns.Add(col);
                }
            }

            while (reader.Read())
            {
                DataRow newRow = tbReturn.NewRow();
                for (int i = 0; i < tbReturn.Columns.Count; i++)
                {
                    newRow[i] = tbReturn.Columns.Count == 1
                        ? (byte[])reader[i]
                        : reader.GetValue(i);
                }
                tbReturn.Rows.Add(newRow);
            }

            return tbReturn;
        }

        public static bool IsValidTableName(string name) => ValidTableNames.Contains(name);
    }
}
