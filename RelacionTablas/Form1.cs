using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RelacionTablas
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        // Cadena de conexión a la base de datos
        string connectionString = "Data Source=GUARI-PC\\SQLEXPRESS;Initial Catalog=Prueba_Dev;Integrated Security=True;";
        private int selectedUserId = -1;

        public void DisplayTableData()
        {
            try
            {
                // Consulta SQL para obtener datos de ambas tablas usando JOIN
                string query = "SELECT Users.id, Users.Nombre, Users.Apellido, Users.Correo, Details.user_id, Details.Edad, Details.Numero " +
                               "FROM Users " +
                               "INNER JOIN Details ON users.id = Details.user_id";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Crear un SqlDataAdapter para llenar un DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    // Llenar el DataTable con los datos de ambas tablas
                    adapter.Fill(dataTable);

                    // Asignar el DataTable como origen de datos del DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores, muestra un mensaje en caso de error
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si se está editando un registro existente o creando uno nuevo
                if (selectedUserId != -1)
                {
                    // Estás editando un registro existente, por lo que debes realizar una actualización en lugar de una inserción

                    // Obtener los nuevos valores de los TextBox
                    string nuevoNombre = textBox1.Text;
                    string nuevoApellido = textBox2.Text;
                    string nuevoCorreo = textBox3.Text;
                    string nuevaEdad = textBox6.Text;
                    string nuevoNumero = textBox5.Text;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Consulta SQL para actualizar los datos del usuario
                        string updateQuery = "UPDATE Users SET Nombre = @Nombre, Apellido = @Apellido, Correo = @Correo WHERE id = @UID";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Nombre", nuevoNombre);
                            command.Parameters.AddWithValue("@Apellido", nuevoApellido);
                            command.Parameters.AddWithValue("@Correo", nuevoCorreo);
                            command.Parameters.AddWithValue("@UID", selectedUserId);

                            command.ExecuteNonQuery();
                        }

                        // Consulta SQL para actualizar el nombre del profesor en la tabla "students"
                        string updateTeacherQuery = "UPDATE Details SET Edad = @Edad, Numero = @Numero WHERE user_id = @UserID";

                        using (SqlCommand teacherCommand = new SqlCommand(updateTeacherQuery, connection))
                        {
                            teacherCommand.Parameters.AddWithValue("@Edad", nuevaEdad);
                            teacherCommand.Parameters.AddWithValue("@Numero", nuevoNumero);
                            teacherCommand.Parameters.AddWithValue("@UserID", selectedUserId);

                            teacherCommand.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Datos actualizados correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar y deshabilitar los TextBox después de guardar
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox5.Clear();
                    textBox6.Clear();

                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox6.Enabled = false;
                    textBox5.Enabled = false;

                    // Reiniciar la variable selectedUserId
                    selectedUserId = -1;

                    // Actualizar la vista de datos en el DataGridView
                    DisplayTableData();
                   
                }
                else
                {

                    try
                    {
                        // Insertar un nuevo usuario en la tabla "users"
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Consulta SQL para insertar un nuevo usuario
                            string insertUserQuery = "INSERT INTO Users (Nombre, Apellido, Correo) VALUES (@Nombre, @Apellido, @Correo); SELECT SCOPE_IDENTITY();";

                            using (SqlCommand command = new SqlCommand(insertUserQuery, connection))
                            {
                                // Pasar los valores de TextBox como parámetros
                                command.Parameters.AddWithValue("@Nombre", textBox1.Text);
                                command.Parameters.AddWithValue("@Apellido", textBox2.Text);
                                command.Parameters.AddWithValue("@Correo", textBox3.Text);

                                // Obtener el ID generado automáticamente
                                int userId = Convert.ToInt32(command.ExecuteScalar());

                                // Insertar el nombre del profesor en la tabla "teachers" con el ID de usuario
                                string insertTeacherQuery = "INSERT INTO Details (user_id, Edad, Numero) VALUES (@UserId, @Edad, @Numero);";

                                using (SqlCommand teacherCommand = new SqlCommand(insertTeacherQuery, connection))
                                {
                                    teacherCommand.Parameters.AddWithValue("@UserId", userId);
                                    teacherCommand.Parameters.AddWithValue("@Edad", textBox6.Text);
                                    teacherCommand.Parameters.AddWithValue("@Numero", textBox5.Text);

                                    // Ejecutar la inserción del profesor
                                    teacherCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        // Deshabilitar los TextBox después de guardar
                        textBox1.Enabled = false;   //nom
                        textBox2.Enabled = false;   //ape
                        textBox3.Enabled = false;   //corr
                        textBox6.Enabled = false;   //edad
                        textBox5.Enabled = false;   //num

                        // Limpiar los TextBox
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox6.Clear();
                        textBox5.Clear();

                        DisplayTableData();

                        MessageBox.Show("Datos guardados correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                    }
                    catch (Exception ex)
                    {
                        // Manejo de errores, muestra un mensaje en caso de error
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            catch (Exception ex)
            {
                // Manejo de errores, muestra un mensaje en caso de error
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Deshabilitar los TextBox
            textBox1.Enabled = false;   //nombre
            textBox2.Enabled = false;   //ape
            textBox3.Enabled = false;   //corr
            textBox5.Enabled = false;   //numero
            textBox6.Enabled = false;   //edad

            // Limpiar los TextBox
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox5.Clear();
            textBox6.Clear();

            DisplayTableData();
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si se ha seleccionado una fila en el DataGridView
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Mostrar un cuadro de diálogo de confirmación
                    DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // Verificar la respuesta del usuario
                    if (result == DialogResult.Yes)
                    {
                        // Obtener el ID del usuario seleccionado desde la fila seleccionada
                        int selectedUserId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Consulta SQL para eliminar el registro de la tabla "teachers"
                            string deleteTeacherQuery = "DELETE FROM Details WHERE user_id = @UserID";

                            using (SqlCommand teacherCommand = new SqlCommand(deleteTeacherQuery, connection))
                            {
                                teacherCommand.Parameters.AddWithValue("@UserID", selectedUserId);
                                teacherCommand.ExecuteNonQuery();
                            }

                            // Consulta SQL para eliminar el registro de la tabla "users"
                            string deleteUserQuery = "DELETE FROM Users WHERE id = @UID";

                            using (SqlCommand userCommand = new SqlCommand(deleteUserQuery, connection))
                            {
                                userCommand.Parameters.AddWithValue("@UID", selectedUserId);
                                userCommand.ExecuteNonQuery();
                            }

                            MessageBox.Show("Registro eliminado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Actualizar la vista de datos en el DataGridView después de eliminar
                            DisplayTableData();
                            
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un registro para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores, muestra un mensaje en caso de error
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Habilita los TextBox para ingresar datos
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;

            // Limpia los TextBox
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox5.Clear();
            textBox6.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Verificar si se ha seleccionado una fila en el DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener el ID del usuario seleccionado desde la fila seleccionada
                selectedUserId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

                // Cargar los valores del usuario en los TextBox para editar
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["nombre"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["apellido"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["correo"].Value.ToString();
                textBox5.Text = dataGridView1.SelectedRows[0].Cells["numero"].Value.ToString();
                textBox6.Text = dataGridView1.SelectedRows[0].Cells["edad"].Value.ToString();

                // Habilitar los TextBox para editar
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un registro para modificar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
