using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rental_BOok
{
    public class DB_con
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        bool con = false;
        string con_info;

        int queryTryNum = 0; // 재시도

        public void connect(string database, string uid, string pwd, string port, string server)
        {
            string strConn_main = "Server={4};Database={0};Uid={1};Pwd={2};Port={3}";

            string strConn = string.Format(strConn_main, database, uid, pwd, port, server);
            con_info = strConn;
            conn = new MySqlConnection(strConn);

            conn.Open();

            //MysqlCommand set
            cmd = new MySqlCommand();
            //cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;
            cmd.CommandTimeout = 2000;

            con = true;
        }

        private void exception_reconnect(string con_info)
        {
            queryTryNum++;
            conn = new MySqlConnection(con_info);

            conn.Open();

            //MysqlCommand set
            cmd = new MySqlCommand();
            //cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;
            cmd.CommandTimeout = 2000;

            con = true;
        }

        public void write_query(string query)
        {
            try
            {
                if (con)
                {
                    // write query
                    string insert = query;
                    cmd.CommandText = insert;
                    cmd.ExecuteNonQuery();
                    queryTryNum = 0;
                }
                else
                {

                }
            }
            catch
            {
                if (queryTryNum < 5)//  연결 재시도시
                {
                    con = false;

                    exception_reconnect(con_info);
                    write_query(query);
                }
                else
                {
                    string insert = query;
                    cmd.CommandText = insert;
                    cmd.ExecuteNonQuery();
                    queryTryNum = 0;
                }
            }

        }

        public string read_str(string query)
        {
            string str = "";
            if (con)
            {
                try
                {
                    cmd.CommandText = query;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = conn;


                    str = cmd.ExecuteScalar().ToString();
                }
                catch
                {
                    str = re_read_str(query); // 결과값이 없거나 연결 재시도시
                }
            }

            queryTryNum = 0;
            return str;
        }

        private string re_read_str(string query)
        {
            string str = "";
            if (con)
            {
                try
                {
                    cmd.CommandText = query;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = conn;

                    str = cmd.ExecuteScalar().ToString();
                }
                catch // 결과 row가 없을경우
                {
                    str = "";
                }

            }

            return str;
        }

        public void Insert(string query)
        {
            try
            {
                cmd.CommandText = query;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                queryTryNum = 0;
            }
            catch
            {
                if (queryTryNum < 5)//  연결 재시도시
                {
                    con = false;

                    exception_reconnect(con_info);
                    Insert(query);
                }
                else
                {
                    cmd.CommandText = query;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                    queryTryNum = 0;
                }
            }
        }

        public DataTable GetTable(string query)
        {
            try
            {

                cmd.CommandText = query;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection = conn;
                DataTable test = new DataTable();

                new MySqlDataAdapter(cmd).Fill(test); // test datatable에 값 넣기

                queryTryNum = 0;
                return test;
            }
            catch
            {
                if(queryTryNum < 5)//  연결 재시도시
                {
                    con = false;

                    exception_reconnect(con_info);
                    return GetTable(query);
                }
                else
                {
                    cmd.CommandText = query;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = conn;
                    DataTable test = new DataTable();

                    new MySqlDataAdapter(cmd).Fill(test); // test datatable에 값 넣기

                    queryTryNum = 0;
                    return test;
                }
            }
        }

    }
}
