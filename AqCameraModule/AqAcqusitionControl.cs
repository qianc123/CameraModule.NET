﻿//***************************************************************
// 文件名（File Name）：    AqAcqusitionImage.cs
//
// 数据表（Tables）：       Nothing
//
// 作者（Author）：         台琰
//
// 日期（Create Date）：    2018.12.04
//
//***************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using AqCameraModule;
using AqDevice;

namespace AqVision.Acquisition
{
    public partial class AqAcqusitionControl : UserControl
    {
        AqAcquisitionImage _acquisitionImage = new AqAcquisitionImage();
        public AqAcquisitionImage AcquisitionImage
        {
            get => _acquisitionImage;
            set => _acquisitionImage = value;
        }

        public AqAcqusitionControl()
        {
            InitializeComponent();
            InitializationControlShow();
            radioButtonCamera_CheckedChanged(null, null);
        }

        private void InitializationControlShow()
        {
            ReArrangeComboBoxCameraName();
            ReArrangeComboBoxCameraBrand();
            ReArrangeComboBoxFile();
            ReArrangeComboBoxFolder();
            //注册显示
            AcquisitionImage.EventOnBitmap += new DelegateOnBitmap(OnGetBitmap);
        }

        /* 回调，参数设置界面点击应用或保存时触发.
         * 有更简单的解决方案，设置界面使用带AqCameraParameters参数类型的构造函数
         * 因为类类型为引用类型，在设置界面只声名对象，而不进行new操作
         * 则在修改参数后，对应this.CameraParam也发生改变
         * 此时该回调则不需传递数值，只需触发操作指令即可
         */
        private void OnCameraParamChanged(AqCameraParameters cameraParam)
        {
            AcquisitionImage.CameraParam = cameraParam;
        }

        //回调，采集到图像时触发
        private void OnGetBitmap(Bitmap bitmap)
        {
            pictureBoxImageShow.Image = bitmap;
        }

        #region 选择图像采集源
        #region From Camera
        private void radioButtonCamera_CheckedChanged(object sender, EventArgs e)
        {
            panelCamera.Enabled = true;
            panelCamerapanelLocalFile.Enabled = false;
            panelLocalFolder.Enabled = false;
            panelAcquisitionCtrl.Enabled = true;
        }

        private void buttonParameterSet_Click(object sender, EventArgs e)
        {
            AqCameraParametersSet CameraParamSet = new AqCameraParametersSet();
            CameraParamSet.CameraParam = AcquisitionImage.CameraParam;
            CameraParamSet.CameraparamChanged += new CameraParamChangedHandler(OnCameraParamChanged);
            CameraParamSet.Show();
            CameraParamSet.Focus();
        }

        private void ReArrangeComboBoxCameraName()
        {
            comboBoxCameraName.Items.Clear();
            for (int i = 0; i < AcquisitionImage.CameraParam.CameraName.Count; i++)
            {
                comboBoxCameraName.Items.Add(AcquisitionImage.CameraParam.CameraName[i]);
            }
            comboBoxCameraName.SelectedIndex = 0;
        }

        private void ReArrangeComboBoxCameraBrand()
        {
            comboBoxCameraBrand.SelectedIndex = 0;
        }
        #endregion

        #region From File
        private void radioButtonLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            panelCamera.Enabled = false;
            panelCamerapanelLocalFile.Enabled = true;
            panelLocalFolder.Enabled = false;
            panelAcquisitionCtrl.Enabled = false;
        }

        private void buttonDeleteFile_Click(object sender, EventArgs e)
        {
            int index = comboBoxFile.SelectedIndex;
            if (index >= 0)
            {
                AcquisitionImage.FileParam.FilePath.RemoveAt(index);
                AcquisitionImage.FileParam.SerializeAndSave();
                ReArrangeComboBoxFile();
            }
        }

        private void buttonLocationDirectory_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "选择输入文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (comboBoxFile.Text == "新增文件") 
                {
                    AcquisitionImage.FileParam.FilePath.Add(dialog.FileName);
                }
                else
                {
                    int index = comboBoxFile.SelectedIndex;
                    AcquisitionImage.FileParam.FilePath[index] = dialog.FileName;
                }
            }
            AcquisitionImage.FileParam.SerializeAndSave();
            ReArrangeComboBoxFile();
        }

        private void ReArrangeComboBoxFile()
        {
            comboBoxFile.Items.Clear();
            for (int i = 0; i < AcquisitionImage.FileParam.FilePath.Count; i++)
            {
                comboBoxFile.Items.Add(AcquisitionImage.FileParam.FilePath[i]);
            }
            comboBoxFile.Items.Add("新增文件");
            comboBoxFile.SelectedIndex = 0;
        }
        #endregion

        #region From Folder
        private void radioButtonLocalFolder_CheckedChanged(object sender, EventArgs e)
        {
            panelCamera.Enabled = false;
            panelCamerapanelLocalFile.Enabled = false;
            panelLocalFolder.Enabled = true;
            panelAcquisitionCtrl.Enabled = false;
        }

        private void buttonDeleteFolder_Click(object sender, EventArgs e)
        {
            int index = comboBoxFolder.SelectedIndex;
            if (index >= 0)
            {
                AcquisitionImage.FileParam.FolderPath.RemoveAt(index);
                AcquisitionImage.FileParam.SerializeAndSave();
                ReArrangeComboBoxFolder();
            }
        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择所有文件存放目录";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                if (comboBoxFolder.Text == "新增文件夹")
                {
                    AcquisitionImage.FileParam.FolderPath.Add(folder.SelectedPath);
                }
                else
                {
                    int index = comboBoxFolder.SelectedIndex;
                    AcquisitionImage.FileParam.FolderPath[index] = folder.SelectedPath;
                }
            }
            AcquisitionImage.FileParam.SerializeAndSave();
            ReArrangeComboBoxFolder();
        }

        private void ReArrangeComboBoxFolder()
        {
            comboBoxFolder.Items.Clear();
            for (int i = 0; i < AcquisitionImage.FileParam.FolderPath.Count; i++)
            {
                comboBoxFolder.Items.Add(AcquisitionImage.FileParam.FolderPath[i]);
            }
            comboBoxFolder.Items.Add("新增文件夹");
            comboBoxFolder.SelectedIndex = 0;
        }
        #endregion

        #endregion

        #region 相机控制按钮
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxCameraBrand.SelectedIndex == -1)
            {
                MessageBox.Show("未选择相机品牌", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AcquisitionImage.OpenAllCamera();
        }

        private void buttonSaveImage_Click(object sender, EventArgs e)
        {

        }

        private void buttonSingle_Click(object sender, EventArgs e)
        {
            if (!AcquisitionImage.IsContinue)
            {
                AcquisitionImage.CameraParam.CameraTriggerMode[comboBoxCameraName.Text] = TriggerModes.Unknow;
                AcquisitionImage.OpenOneStream(comboBoxCameraName.SelectedIndex);
            }
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if (AcquisitionImage.IsContinue)
            {
                buttonContinue.Text = "连续采集";
                AcquisitionImage.CameraParam.CameraTriggerMode[comboBoxCameraName.Text] = TriggerModes.Unknow;
            }
            else
            {
                buttonContinue.Text = "停止采集";
                AcquisitionImage.CameraParam.CameraTriggerMode[comboBoxCameraName.Text] = TriggerModes.Continuous;
            }
            AcquisitionImage.OpenOneStream(comboBoxCameraName.SelectedIndex);
        }
        #endregion
    }
}
