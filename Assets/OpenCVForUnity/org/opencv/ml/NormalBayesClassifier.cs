
//
// This file is auto-generated. Please don't modify it!
//
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace OpenCVForUnity
{

// C++: class NormalBayesClassifier
//javadoc: NormalBayesClassifier
		public class NormalBayesClassifier : StatModel
		{
				protected override void Dispose (bool disposing)
				{
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5
try {
if (disposing) {
}
if (IsEnabledDispose) {
if (nativeObj != IntPtr.Zero)
ml_NormalBayesClassifier_delete(nativeObj);
nativeObj = IntPtr.Zero;
}
} finally {
base.Dispose (disposing);
}
#else
						return;
#endif
				}

				protected NormalBayesClassifier (IntPtr addr) : base(addr)
				{
				}


				//
				// C++: static Ptr_NormalBayesClassifier create()
				//

				//javadoc: NormalBayesClassifier::create()
				public static NormalBayesClassifier create ()
				{
#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        NormalBayesClassifier retVal = new NormalBayesClassifier(ml_NormalBayesClassifier_create_10());
        
        return retVal;
#else
						return null;
#endif
				}


				//
				// C++:  float predictProb(Mat inputs, Mat& outputs, Mat& outputProbs, int flags = 0)
				//

				//javadoc: NormalBayesClassifier::predictProb(inputs, outputs, outputProbs, flags)
				public  float predictProb (Mat inputs, Mat outputs, Mat outputProbs, int flags)
				{
						ThrowIfDisposed ();
						if (inputs != null)
								inputs.ThrowIfDisposed ();
						if (outputs != null)
								outputs.ThrowIfDisposed ();
						if (outputProbs != null)
								outputProbs.ThrowIfDisposed ();

#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        float retVal = ml_NormalBayesClassifier_predictProb_10(nativeObj, inputs.nativeObj, outputs.nativeObj, outputProbs.nativeObj, flags);
        
        return retVal;
#else
						return -1;
#endif
				}

				//javadoc: NormalBayesClassifier::predictProb(inputs, outputs, outputProbs)
				public  float predictProb (Mat inputs, Mat outputs, Mat outputProbs)
				{
						ThrowIfDisposed ();
						if (inputs != null)
								inputs.ThrowIfDisposed ();
						if (outputs != null)
								outputs.ThrowIfDisposed ();
						if (outputProbs != null)
								outputProbs.ThrowIfDisposed ();

#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR) || UNITY_5

        
        float retVal = ml_NormalBayesClassifier_predictProb_11(nativeObj, inputs.nativeObj, outputs.nativeObj, outputProbs.nativeObj);
        
        return retVal;
#else
						return -1;
#endif
				}


    
		#if UNITY_IOS && !UNITY_EDITOR
		const string LIBNAME = "__Internal";
		#else
				const string LIBNAME = "opencvforunity";
		#endif


				// C++: static Ptr_NormalBayesClassifier create()
				[DllImport(LIBNAME)]
				private static extern IntPtr ml_NormalBayesClassifier_create_10 ();

				// C++:  float predictProb(Mat inputs, Mat& outputs, Mat& outputProbs, int flags = 0)
				[DllImport(LIBNAME)]
				private static extern float ml_NormalBayesClassifier_predictProb_10 (IntPtr nativeObj, IntPtr inputs_nativeObj, IntPtr outputs_nativeObj, IntPtr outputProbs_nativeObj, int flags);

				[DllImport(LIBNAME)]
				private static extern float ml_NormalBayesClassifier_predictProb_11 (IntPtr nativeObj, IntPtr inputs_nativeObj, IntPtr outputs_nativeObj, IntPtr outputProbs_nativeObj);

				// native support for java finalize()
				[DllImport(LIBNAME)]
				private static extern void ml_NormalBayesClassifier_delete (IntPtr nativeObj);

		}
}
