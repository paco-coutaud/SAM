using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Java.Lang;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware.Camera2;
using Android.Util;
using Android.Hardware.Camera2.Params;
using Android.Graphics;

/*Min API lvl 21*/

namespace InterphoneSAM
{
    class CameraFragment : Fragment
    {
        private static readonly SparseIntArray ORIENTATIONS = new SparseIntArray();
        private CameraDevice cameraDevice;
        private AutoFitTextureView CameraTextureView;
        private CameraSurfaceTextureListener surfaceTextureListener;
        private CaptureRequest.Builder PreviewBuilder;
        private CameraCaptureSession PreviewSession;
        private class CameraSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
        {
            private CameraFragment Fragment;

            public CameraSurfaceTextureListener(CameraFragment fragment)
            {
                Fragment = fragment;
            }
            public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
            {
                Fragment.ConfigureTransform(width, height);
                Fragment.StartPreview();
            }

            public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
            {
                return true;
            }

            public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
            {
                Fragment.ConfigureTransform(width, height);
                Fragment.StartPreview();
            }

            public void OnSurfaceTextureUpdated(SurfaceTexture surface)
            {
            }
        }

        private Size previewSize;
        private bool openingCamera;

        private CameraStateListener StateListener;

        private class CameraStateListener : CameraDevice.StateCallback
        {

            public CameraFragment Fragment;
            public override void OnDisconnected(CameraDevice camera)
            {
                if (Fragment != null)
                {
                    camera.Close();
                    Fragment.cameraDevice = null;
                    Fragment.openingCamera = false;
                }
            }

            public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
            {
                camera.Close();
                if (Fragment != null)
                {
                    Fragment.cameraDevice = null;
                    Activity activity = Fragment.Activity;
                    Fragment.openingCamera = false;
                    if (activity != null)
                    {
                        activity.Finish();
                    }
                }
            }

            public override void OnOpened(CameraDevice camera)
            {
                if (Fragment != null)
                {
                    Fragment.cameraDevice = camera;
                    Fragment.StartPreview();
                    Fragment.openingCamera = false;
                }
            }
        }


        private class CameraCaptureStateListener : CameraCaptureSession.StateCallback
        {

            public Action<CameraCaptureSession> OnConfiguredAction;
            public override void OnConfigured(CameraCaptureSession session)
            {
                if (OnConfiguredAction != null)
                {
                    OnConfiguredAction(session);
                }
            }

            public Action<CameraCaptureSession> OnConfigureFailedAction;
            public override void OnConfigureFailed(CameraCaptureSession session)
            {
                if (OnConfigureFailedAction != null)
                {
                    OnConfigureFailedAction(session);
                }
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StateListener = new CameraStateListener() { Fragment = this };
            surfaceTextureListener = new CameraSurfaceTextureListener(this);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation0, 90);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation90, 0);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation180, 270);
            ORIENTATIONS.Append((int)SurfaceOrientation.Rotation270, 180);
        }

        public static CameraFragment NewInstance()
        {
            CameraFragment fragment = new CameraFragment();
            fragment.RetainInstance = true;
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_camera, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            CameraTextureView = (AutoFitTextureView)view.FindViewById(Resource.Id.texture);
            CameraTextureView.SurfaceTextureListener = surfaceTextureListener;

        }

        public override void OnResume()
        {
            base.OnResume();
            OpenCamera();
        }

        public override void OnPause()
        {
            base.OnPause();
            if (cameraDevice != null)
            {
                cameraDevice.Close();
                cameraDevice = null;
            }
        }

        private void OpenCamera()
        {
            Activity activity = Activity;
            if (activity == null || activity.IsFinishing || openingCamera)
            {
                return;
            }

            openingCamera = true;
            CameraManager manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            try
            {
                string cameraId = manager.GetCameraIdList()[1];
                CameraCharacteristics Characteristic = manager.GetCameraCharacteristics(cameraId);
                StreamConfigurationMap map = (StreamConfigurationMap)Characteristic.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                previewSize = map.GetOutputSizes(Java.Lang.Class.FromType(typeof(SurfaceTexture)))[0];
                Android.Content.Res.Orientation orientation = Resources.Configuration.Orientation;
                if (orientation == Android.Content.Res.Orientation.Landscape)
                {
                    CameraTextureView.SetAspectRatio(previewSize.Width, previewSize.Height);

                }
                else
                {
                    CameraTextureView.SetAspectRatio(previewSize.Height, previewSize.Width);
                }

                manager.OpenCamera(cameraId, StateListener, null);
            }
            catch (CameraAccessException ex)
            {
                Toast.MakeText(activity, "Cannot access the camera.", ToastLength.Short).Show();
                activity.Finish();
            }
            catch (NullPointerException) {
				var dialog = new ErrorDialog();
                dialog.Show (FragmentManager, "dialog");
			}

}

        private void StartPreview()
        {
            if (cameraDevice == null || !CameraTextureView.IsAvailable || previewSize == null)
            {
                return;
            }
            try
            {
                SurfaceTexture texture = CameraTextureView.SurfaceTexture;
                System.Diagnostics.Debug.Assert(texture != null);
                texture.SetDefaultBufferSize(previewSize.Width, previewSize.Height);
                Surface surface = new Surface(texture);
                PreviewBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                PreviewBuilder.AddTarget(surface);
                cameraDevice.CreateCaptureSession(new List<Surface>() { surface },
                    new CameraCaptureStateListener()
                    {
                        OnConfigureFailedAction = (CameraCaptureSession session) =>
                        {
                            Activity activity = Activity;
                            if (activity != null)
                            {
                                Toast.MakeText(activity, "Failed", ToastLength.Short).Show();

                            }

                        },

                        OnConfiguredAction = (CameraCaptureSession session) =>
                        {
                            PreviewSession = session;
                            UpdatePreview();
                        }
                    },
                    null);


            } catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "CameraFragment", ex.StackTrace);
            }
        }
        private void UpdatePreview()
        {
            if (cameraDevice == null)
            {
                return;
            }
            try
            {
                SetUpCaptureRequestBuilder(PreviewBuilder);
                HandlerThread thread = new HandlerThread("CameraPreview");
                thread.Start();
                Handler backgroundHandler = new Handler(thread.Looper);
                PreviewSession.SetRepeatingRequest(PreviewBuilder.Build(), null, backgroundHandler);

            } catch (CameraAccessException ex)
            {
                Log.WriteLine(LogPriority.Info, "CameraFragment", ex.StackTrace);
             }
        }
        private void SetUpCaptureRequestBuilder(CaptureRequest.Builder builder)
        {
            builder.Set(CaptureRequest.ControlMode, new Integer((int)ControlMode.Auto));
        }

        private void ConfigureTransform(int viewWidth, int viewHeight)
        {
            Activity activity = Activity;
            if (CameraTextureView == null || previewSize == null || activity == null)
            {
                return;
            }

            SurfaceOrientation rotation = activity.WindowManager.DefaultDisplay.Rotation;
            Matrix matrix = new Matrix();
            RectF viewRect = new RectF(0, 0, viewWidth, viewHeight);
            RectF bufferRect = new RectF(0, 0, previewSize.Width, previewSize.Height);
            float centerX = viewRect.CenterX();
            float centerY = viewRect.CenterY();
            if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
            {
                bufferRect.Offset(centerX - bufferRect.CenterX(), centerY - bufferRect.CenterY());
                matrix.SetRectToRect(viewRect, bufferRect, Matrix.ScaleToFit.Fill);
                float scale = System.Math.Max((float)viewHeight / previewSize.Height, (float)viewWidth / previewSize.Width);
                matrix.PostScale(scale, scale, centerX, centerY);
                matrix.PostRotate(90 * ((int)rotation - 2), centerX, centerY);

            }
            CameraTextureView.SetTransform(matrix);
        }
        public class ErrorDialog : DialogFragment
        {
            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                var alert = new AlertDialog.Builder(Activity);
                alert.SetMessage("This device doesn't support Camera2 API.");
                return alert.Show();

            }
        }
    }
}