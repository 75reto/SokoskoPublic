using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sokoban_Programm
{
    /// <summary>
    /// Interaction logic for UCCell.xaml
    /// </summary>
    public partial class UCCell : UserControl
    {
        public UCCell()
        {
            InitializeComponent();
        }

        public int playerDirection;

        public void ClearAll()
        {
            ImgCoin.Visibility = Visibility.Hidden;
            ImgCrate.Visibility = Visibility.Hidden;
            ImgGround.Visibility = Visibility.Hidden;
            ImgPlayerRight.Visibility = Visibility.Hidden;
            ImgPlayer_Down.Visibility = Visibility.Hidden;
            ImgPlayer_Left.Visibility = Visibility.Hidden;
            ImgPLayer_Up.Visibility = Visibility.Hidden;
            ImgTarget.Visibility = Visibility.Hidden;
            ImgWall.Visibility = Visibility.Hidden;
            ImgCrateTarget.Visibility = Visibility.Hidden;
        }


        public void WidthHeightUpdate(int widthAndHeight)
        {
            wndCell.Width = widthAndHeight;
            wndCell.Height = widthAndHeight;

            grdCell.Width = widthAndHeight;
            grdCell.Height = widthAndHeight;
        }

        //Render Wall

        public void Wall()
        {
            ClearAll();
            ImgWall.Visibility = Visibility.Visible;
        }

        //Get player direction
        public void PlayerDirection(int direction)
        {
            playerDirection = direction;
        }

        //Render Target
        public void Target()
        {
            ClearAll();
            ImgTarget.Visibility = Visibility.Visible;
            ImgWall.Visibility = Visibility.Visible;
        }

        //Render Ground
        public void Ground()
        {
            ClearAll();
            ImgGround.Visibility = Visibility.Visible;
        }

        //Set player Direction
        public void Player()
        {
            if (playerDirection == 1)
            {
                ImgPlayerRight.Visibility = Visibility.Visible;
            }
            else if (playerDirection == 2)
            {
                ImgPlayer_Left.Visibility = Visibility.Visible;
            }
            else if (playerDirection == 3)
            {
                ImgPlayer_Down.Visibility = Visibility.Visible;
            }
            else if (playerDirection == 4)
            {
                ImgPLayer_Up.Visibility = Visibility.Visible;
            } 
            
        }

        //Render Player on Ground
        public void PlayerGround()
        {
            ClearAll();
            Ground();
            Player();
        }

        //Render Crate
        public void Crate()
        {
            ClearAll();
            ImgCrate.Visibility = Visibility.Visible;
        }

        //Render Crate on Target
        public void CrateTarget()
        {
            ClearAll();
            ImgCrateTarget.Visibility = Visibility.Visible;
        }

        //Render Player on Target
        public void PlayerTarget()
        {
            ClearAll();
            Target();
            Player();
        }

    }
}
