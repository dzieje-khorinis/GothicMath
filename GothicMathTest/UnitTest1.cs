using System;
using Xunit;
using GothicMathLib;
using OpenTK;
namespace GothicMathTest
{
    public class UnitTest1
    {
        [Fact]
        public void testMathAngle()
        {
            var zenMathReturn    = GothicMath.getAngle(Matrix4.Zero);
            var expect           = new Vector4(0, 0, 0, (float)(120 * (Math.PI / 180)));

            Assert.Equal(zenMathReturn, expect);
        }

        [Fact]
        public void testEulerAnglesFromMatrix3()
        {
            var zenRet = GothicMath.getEulerAngle(Matrix3.Zero);
            var expect = new Vector3(0, 0, 0);

            Assert.Equal(zenRet, expect);
        }

        [Fact]
        public void testMatrixFromEulerAngle()
        {
            var zenRet = GothicMath.getRotateMatrixFromEulerAngle(Vector3.One);
            var expect = Matrix4.Identity;

            expect.M11 = 0.88775f   ;
            expect.M12 = -0.072075f ;
            expect.M13 = -0.454649f ;

            expect.M21 = 0.454649f;
            expect.M22 = 0.291927f;
            expect.M23 = 0.841471f;

            expect.M31 = 0.072075f;
            expect.M32 = -0.953721f;
            expect.M33 = 0.291927f;

            // rounding numbers for proper operation
			for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    zenRet[i, j] = MathF.Round(zenRet[i, j], 5);
                    expect[i, j] = MathF.Round(expect[i, j], 5);
                }
            }

            Assert.Equal(zenRet, expect);
        }

        [Fact]
        public void testExtractRotationToEulerAngles()
        {
            var zenRet = GothicMath.rotationMatrixToEulerAngles(Matrix4.Zero);
            var expect = new Vector3(0, 0, 0);

            Assert.Equal(zenRet, expect);
        }

        [Fact]
        public void testVectorDistance()
        {
            var zenRet = GothicMath.getDistance(new Vector3(0), Vector3.One);
            var expect = 3;

            Assert.Equal(zenRet, expect);
        }

        [Fact]
        public void testCalculateMove()
        {
            var zenRet = GothicMath.CalcMoveObject(Vector3.Zero, 0.2f, 0.5f);
            var expect = Vector3.Zero;

            Assert.Equal(zenRet, expect);
        }

        [Fact]
        public void testCalculateNormal()  
        {
            var zenRet = GothicMath.CalcNormal(Vector3.Zero, new Vector3(120, 20, 50), new Vector3(56, 90, -20));
            var dir = new Vector3(-4900, 5200, 9680);
            var expect = Vector3.Normalize(dir);

            Assert.Equal(zenRet, expect);
        }

        [Fact]
        public void testCalculateScale()
        {
            var zenRet = GothicMath.CalcSizeModel(Vector3.Zero, Vector3.One);
            var expect = Vector3.One;

            Assert.Equal(zenRet, expect);
        }


        [Fact]
        public void testCalculateCenter()
        {
            var zenRet = GothicMath.CalcCenterModel(Vector3.Zero, Vector3.One);
            var expect = Vector3.One / 2;

            Assert.Equal(zenRet, expect);
        }

    }
}