using System;
using System.Windows.Forms;

namespace ShutdownTimerWin32
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Show license if not already shown
            if (Properties.Settings.Default.LicenseShown == false)
            {
                MessageBox.Show("THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE." +
                    "\n\nBy using this software you agree to the above mentioned terms as this software is licensed under the MIT License. For more information visit: https://opensource.org/licenses/MIT.", "MIT License", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.LicenseShown = true; // user has seen license prompt
                Properties.Settings.Default.Save(); // save settings
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu());
        }
    }
}
