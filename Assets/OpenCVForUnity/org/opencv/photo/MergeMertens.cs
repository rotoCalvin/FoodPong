
//
// This file is auto-generated. Please don't modify it!
//
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace OpenCVForUnity
{

// C++: class MergeMertens
//javadoc: MergeMertens
		public class MergeMertens : MergeExposures
		{
				protected override void Dispose (bool disposing)
				{
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
try {
if (disposing) {
}
if (IsEnabledDispose) {
if (nativeObj != IntPtr.Zero)
photo_MergeMertens_delete(nativeObj);
nativeObj = IntPtr.Zero;
}
} finally {
base.Dispose (disposing);
}
#else
						return;
#endif
				}

				public MergeMertens (IntPtr addr) : base(addr)
				{
				}


				//
				// C++:  float getContrastWeight()
				//

				//javadoc: MergeMertens::getContrastWeight()
				public  float getContrastWeight ()
				{
						ThrowIfDisposed ();
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        float retVal = photo_MergeMertens_getContrastWeight_10(nativeObj);
        
        return retVal;
#else
						return -1;
#endif
				}


				//
				// C++:  float getExposureWeight()
				//

				//javadoc: MergeMertens::getExposureWeight()
				public  float getExposureWeight ()
				{
						ThrowIfDisposed ();
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        float retVal = photo_MergeMertens_getExposureWeight_10(nativeObj);
        
        return retVal;
#else
						return -1;
#endif
				}


				//
				// C++:  float getSaturationWeight()
				//

				//javadoc: MergeMertens::getSaturationWeight()
				public  float getSaturationWeight ()
				{
						ThrowIfDisposed ();
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        float retVal = photo_MergeMertens_getSaturationWeight_10(nativeObj);
        
        return retVal;
#else
						return -1;
#endif
				}


				//
				// C++:  void process(vector_Mat src, Mat& dst, Mat times, Mat response)
				//

				//javadoc: MergeMertens::process(src, dst, times, response)
				public override void process (List<Mat> src, Mat dst, Mat times, Mat response)
				{
						ThrowIfDisposed ();
						if (dst != null)
								dst.ThrowIfDisposed ();
						if (times != null)
								times.ThrowIfDisposed ();
						if (response != null)
								response.ThrowIfDisposed ();

#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        Mat src_mat = Converters.vector_Mat_to_Mat(src);
        photo_MergeMertens_process_10(nativeObj, src_mat.nativeObj, dst.nativeObj, times.nativeObj, response.nativeObj);
        
        return;
#else
						return;
#endif
				}


				//
				// C++:  void process(vector_Mat src, Mat& dst)
				//

				//javadoc: MergeMertens::process(src, dst)
				public  void process (List<Mat> src, Mat dst)
				{
						ThrowIfDisposed ();
						if (dst != null)
								dst.ThrowIfDisposed ();

#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        Mat src_mat = Converters.vector_Mat_to_Mat(src);
        photo_MergeMertens_process_11(nativeObj, src_mat.nativeObj, dst.nativeObj);
        
        return;
#else
						return;
#endif
				}


				//
				// C++:  void setContrastWeight(float contrast_weiht)
				//

				//javadoc: MergeMertens::setContrastWeight(contrast_weiht)
				public  void setContrastWeight (float contrast_weiht)
				{
						ThrowIfDisposed ();
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        photo_MergeMertens_setContrastWeight_10(nativeObj, contrast_weiht);
        
        return;
#else
						return;
#endif
				}


				//
				// C++:  void setExposureWeight(float exposure_weight)
				//

				//javadoc: MergeMertens::setExposureWeight(exposure_weight)
				public  void setExposureWeight (float exposure_weight)
				{
						ThrowIfDisposed ();
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        photo_MergeMertens_setExposureWeight_10(nativeObj, exposure_weight);
        
        return;
#else
						return;
#endif
				}


				//
				// C++:  void setSaturationWeight(float saturation_weight)
				//

				//javadoc: MergeMertens::setSaturationWeight(saturation_weight)
				public  void setSaturationWeight (float saturation_weight)
				{
						ThrowIfDisposed ();
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        photo_MergeMertens_setSaturationWeight_10(nativeObj, saturation_weight);
        
        return;
#else
						return;
#endif
				}


    
		#if UNITY_IOS && !UNITY_EDITOR
		const string LIBNAME = "__Internal";
		#else
				const string LIBNAME = "opencvforunity";
		#endif


				// C++:  float getContrastWeight()
				[DllImport(LIBNAME)]
				private static extern float photo_MergeMertens_getContrastWeight_10 (IntPtr nativeObj);

				// C++:  float getExposureWeight()
				[DllImport(LIBNAME)]
				private static extern float photo_MergeMertens_getExposureWeight_10 (IntPtr nativeObj);

				// C++:  float getSaturationWeight()
				[DllImport(LIBNAME)]
				private static extern float photo_MergeMertens_getSaturationWeight_10 (IntPtr nativeObj);

				// C++:  void process(vector_Mat src, Mat& dst, Mat times, Mat response)
				[DllImport(LIBNAME)]
				private static extern void photo_MergeMertens_process_10 (IntPtr nativeObj, IntPtr src_mat_nativeObj, IntPtr dst_nativeObj, IntPtr times_nativeObj, IntPtr response_nativeObj);

				// C++:  void process(vector_Mat src, Mat& dst)
				[DllImport(LIBNAME)]
				private static extern void photo_MergeMertens_process_11 (IntPtr nativeObj, IntPtr src_mat_nativeObj, IntPtr dst_nativeObj);

				// C++:  void setContrastWeight(float contrast_weiht)
				[DllImport(LIBNAME)]
				private static extern void photo_MergeMertens_setContrastWeight_10 (IntPtr nativeObj, float contrast_weiht);

				// C++:  void setExposureWeight(float exposure_weight)
				[DllImport(LIBNAME)]
				private static extern void photo_MergeMertens_setExposureWeight_10 (IntPtr nativeObj, float exposure_weight);

				// C++:  void setSaturationWeight(float saturation_weight)
				[DllImport(LIBNAME)]
				private static extern void photo_MergeMertens_setSaturationWeight_10 (IntPtr nativeObj, float saturation_weight);

				// native support for java finalize()
				[DllImport(LIBNAME)]
				private static extern void photo_MergeMertens_delete (IntPtr nativeObj);

		}
}
