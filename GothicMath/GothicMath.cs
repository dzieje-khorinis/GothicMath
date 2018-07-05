using System;

namespace GothicMathLib
{
    public static class GothicMath
    {
        // Расчёт луча
        //
        public static void MouseRay(int mouseX, int mouseY, int screenWidth, int screenHeight, Matrix4 ViewMatrix, Matrix4 ProjectionMatrix, out Vector3 _start, out Vector3 _end)
        {
            Vector3 mousePosA = new Vector3(mouseX, mouseY, -1f);
            Vector3 mousePosB = new Vector3(mouseX, mouseY, 1f);

            Matrix4 _projMatrix = ProjectionMatrix;
            Matrix4 _viewMatrix = ViewMatrix;

            Vector4 nearUnproj = UnProject(ref _projMatrix, _viewMatrix, new Size(screenWidth, screenHeight), mousePosA);
            Vector4 farUnproj = UnProject(ref _projMatrix, _viewMatrix, new Size(screenWidth, screenHeight), mousePosB);

            Vector3 dir = farUnproj.Xyz - nearUnproj.Xyz;
            dir.Normalize();

            _start = nearUnproj.Xyz;
            _end = dir;
        }

        private static Vector4 UnProject(ref Matrix4 projection, Matrix4 view, Size viewport, Vector3 mouse)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / viewport.Height - 1);
            vec.Z = mouse.Z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec;
        }

        public static bool RaySphereCollision(Vector3 vSphereCenter, float fSphereRadius, Vector3 vA, Vector3 vB)
        {
            // Вектор из конечной точки vA в центр сферы
            Vector3 vDirToSphere = vSphereCenter - vA;

            // Нормализованный вектор направления из конечной точки vA до конечной точки vB
            Vector3 vLineDir = Vector3.Normalize(vB - vA);

            // Длина сегмента линии
            Vector3 fLine = vA - vB;
            float fLineLength = fLine.Length;

            // Проецирование vDirToSphere на вектор vLineDir
            float t = Vector3.Dot(vDirToSphere, vLineDir);

            Vector3 vClosestPoint;
            // Если проецируемое расстояние от vA <= 0, блишайшая точка равна vA
            if (t <= 0.0f)
                vClosestPoint = vA;
            // Если проецируемое расстояние от vA > длины линии, ближайшей точкой будет vB
            else if (t >= fLineLength)
                vClosestPoint = vB;
            // Иначе вычислите точку на линии с помощью t
            else
                vClosestPoint = vA + vLineDir * t;

            Vector3 res = vSphereCenter - vClosestPoint;

            // Проверка точки (находится ли точка в радиусе сферы)
            return res.Length <= fSphereRadius;
        }

        // Получение угла поворота в виде вектора
        public static Vector4 getAngle(Matrix4 rotate)
        {
            // Сумма элементров главной диагонали
            float traceR = rotate.M11 + rotate.M22 + rotate.M33;

            // Угол поворота 
            double theta = Math.Acos((traceR - 1) * 0.5);

            var omegaPreCalc = 1.0 / (2 * Math.Sin(theta));
            return new Vector4(
                (float)omegaPreCalc * (rotate.M32 - rotate.M23),
                (float)omegaPreCalc * (rotate.M13 - rotate.M31),
                (float)omegaPreCalc * (rotate.M21 - rotate.M12),
                (float)theta
                );
        }

        // Вычисление углов из матрицы вращения
        public static Vector3 getEulerAngle(Matrix3 directx_matrix_rotate)
        {
            Vector3 euler = new Vector3();

            euler.X = (float)Math.Asin(-directx_matrix_rotate.M32);
  
            if (Math.Cos(euler.X) > 0.0001)
            {
                euler.Y = (float)Math.Atan2(directx_matrix_rotate.M31, directx_matrix_rotate.M33);
                euler.Z = (float)Math.Atan2(directx_matrix_rotate.M12, directx_matrix_rotate.M22);
            }
            else
            {
                euler.Y = 0;
                euler.Z = (float)Math.Atan2(-directx_matrix_rotate.M21, directx_matrix_rotate.M11);
            }

            return euler;
        }

        // Формирование матрицы вращения из углов Эйлера
        public static Matrix4 getRotateMatrixFromEulerAngle(Vector3 eulerAngle)
        {
            double cosY = Math.Cos(-eulerAngle.Y);     // Yaw
            double sinY = Math.Sin(-eulerAngle.Y);

            double cosP = Math.Cos(-eulerAngle.X);     // Pitch
            double sinP = Math.Sin(-eulerAngle.X);

            double cosR = Math.Cos(eulerAngle.Z);     // Roll
            double sinR = Math.Sin(eulerAngle.Z);

            Matrix4 rotate = Matrix4.Identity;
            rotate.M11 = (float)(cosY * cosR + sinY * sinP * sinR);
            rotate.M12 = (float)(cosR * sinY * sinP - sinR * cosY);
            rotate.M13 = (float)(cosP * sinY);

            rotate.M21 = (float)(cosP * sinR);
            rotate.M22 = (float)(cosR * cosP);
            rotate.M23 = (float)(-sinP);

            rotate.M31 = (float)(sinR * cosY * sinP - sinY * cosR);
            rotate.M32 = (float)(sinY * sinR + cosR * cosY * sinP);
            rotate.M33 = (float)(cosP * cosY);

            return rotate;
        }

        /* =======================================
              |1    0      0    |
          X = |0    cos    -sin |
              |0    sin    cos  |

              |cos    0    sin  |
          Y = |0      1    0    |
              |-sin   0    cos  |
           
              |cos  -sin     0  |
          Z = |sin   cos     0  |
              |0      0      1  |
          ======================================*/
        // Получение угла поворота в виде вектора
        public static Vector3 rotationMatrixToEulerAngles(Matrix4 rotate)
        {
            double sy = Math.Sqrt(rotate.M11 * rotate.M11 + rotate.M21 * rotate.M21);

            bool singular = sy < 1e-6; // If

            double x, y, z;
            if (!singular)
            {
                x = Math.Atan2(rotate.M32, rotate.M33);
                y = Math.Atan2(-rotate.M31, sy);
                z = Math.Atan2(rotate.M21, rotate.M11);
            }
            else
            {
                x = Math.Atan2(-rotate.M23, rotate.M22);
                y = Math.Atan2(-rotate.M31, sy);
                z = 0;
            }
            
            // degrees = radians × 180° / π
            double p = 57.295779513;

            //return new Vector3((float)(p * x), (float)(y * p), (float)(z * p));    
            return new Vector3((float)x, (float)y, (float)(z));
        }

        // Получение расстояния между двумя векторами
        public static float getDistance(Vector3 dist1, Vector3 dist2)
        {
            return Math.Abs((dist1.X - dist2.X) + (dist1.Y - dist2.Y) + (dist1.Z - dist2.Z));
        }

        public static Vector3 CalcMoveObject(Vector3 position, float cameraOrientationX, float MoveSpeed)
        {
            Vector3 offset = new Vector3();

            Vector3 forward = new Vector3((float)Math.Sin(cameraOrientationX), 0, (float)Math.Cos(cameraOrientationX));
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);

            offset += position.X * right;
            offset += position.Y * forward;
            offset.Y += position.Z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            return offset;
        }

        
        public static Vector3 CalcNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var dir = Vector3.Cross(b - a, c - a);
            var norm = Vector3.Normalize(dir);

            return norm;
        }

        public static Vector3 CalcSizeModel(Vector3 min, Vector3 max)
        {
            return max-min;
        }

        public static Vector3 CalcCenterModel(Vector3 min, Vector3 max)
        {
            return (min+max)/2;
        }
    }
}