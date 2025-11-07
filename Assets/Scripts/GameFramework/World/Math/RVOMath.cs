/*
 * RVOMath.cs
 * RVO2 Library C#
 *
 * Copyright 2008 University of North Carolina at Chapel Hill
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Please send all bug reports to <geom@cs.unc.edu>.
 *
 * The authors may be contacted via:
 *
 * Jur van den Berg, Stephen J. Guy, Jamie Snape, Ming C. Lin, Dinesh Manocha
 * Dept. of Computer Science
 * 201 S. Columbia St.
 * Frederick P. Brooks, Jr. Computer Science Bldg.
 * Chapel Hill, N.C. 27599-3175
 * United States of America
 *
 * <http://gamma.cs.unc.edu/RVO2/>
 */

#if USE_FIXEDMATH
using ExternEngine;
#else
using FFloat = System.Single;
using FVector3 = UnityEngine.Vector3;
#endif
using System;
using UnityEngine;

namespace RVO
{
    /**
     * <summary>Contains functions and constants used in multiple classes.
     * </summary>
     */
    public struct RVOMath
    {
        /**
         * <summary>A sufficiently small positive number.</summary>
         */
        internal static FFloat RVOEPSILON = 0.00001f;

        /**
         * <summary>Computes the length of a specified two-dimensional vector.
         * </summary>
         *
         * <param name="vector">The two-dimensional vector whose length is to be
         * computed.</param>
         * <returns>The length of the two-dimensional vector.</returns>
         */
        public static FFloat abs(FVector3 vector)
        {
            return sqrt(absSq(vector));
        }

        /**
         * <summary>Computes the squared length of a specified two-dimensional
         * vector.</summary>
         *
         * <returns>The squared length of the two-dimensional vector.</returns>
         *
         * <param name="vector">The two-dimensional vector whose squared length
         * is to be computed.</param>
         */
        public static FFloat absSq(FVector3 vector)
        {
            return FVector3.Dot(vector , vector);
        }

        /**
         * <summary>Computes the normalization of the specified two-dimensional
         * vector.</summary>
         *
         * <returns>The normalization of the two-dimensional vector.</returns>
         *
         * <param name="vector">The two-dimensional vector whose normalization
         * is to be computed.</param>
         */
        public static FVector3 normalize(FVector3 vector)
        {
            return vector / abs(vector);
        }

        /**
         * <summary>Computes the determinant of a two-dimensional square matrix
         * with rows consisting of the specified two-dimensional vectors.
         * </summary>
         *
         * <returns>The determinant of the two-dimensional square matrix.
         * </returns>
         *
         * <param name="vector1">The top row of the two-dimensional square
         * matrix.</param>
         * <param name="vector2">The bottom row of the two-dimensional square
         * matrix.</param>
         */
        internal static FFloat det(FVector3 vector1, FVector3 vector2)
        {
            return vector1.x * vector2.z - vector1.z * vector2.x;
        }

        internal static FFloat mul(FVector3 vector1, FVector3 vector2)
        {
            return vector1.x * vector2.z + vector1.z * vector2.x;
        }
        /**
         * <summary>Computes the squared distance from a line segment with the
         * specified endpoints to a specified point.</summary>
         *
         * <returns>The squared distance from the line segment to the point.
         * </returns>
         *
         * <param name="vector1">The first endpoint of the line segment.</param>
         * <param name="vector2">The second endpoint of the line segment.
         * </param>
         * <param name="vector3">The point to which the squared distance is to
         * be calculated.</param>
         */
        internal static FFloat distSqPointLineSegment(FVector3 vector1, FVector3 vector2, FVector3 vector3)
        {
            FFloat r = (FVector3.Dot((vector3 - vector1) , (vector2 - vector1))) / absSq(vector2 - vector1);

            if (r < 0.0f)
            {
                return absSq(vector3 - vector1);
            }

            if (r > 1.0f)
            {
                return absSq(vector3 - vector2);
            }

            return absSq(vector3 - (vector1 + r * (vector2 - vector1)));
        }

        /**
         * <summary>Computes the absolute value of a float.</summary>
         *
         * <returns>The absolute value of the float.</returns>
         *
         * <param name="scalar">The float of which to compute the absolute
         * value.</param>
         */
        internal static FFloat fabs(FFloat scalar)
        {
#if USE_FIXEDMATH
            return FMath.Abs(scalar);
#else
            return Math.Abs(scalar);
#endif
        }

        internal static bool IsZeroXZ(FVector3 speed)
        {
            return Math.Abs(speed.x) <= 0.001f && Math.Abs(speed.z) <= 0.001f;
        }

        /**
         * <summary>Computes the signed distance from a line connecting the
         * specified points to a specified point.</summary>
         *
         * <returns>Positive when the point c lies to the left of the line ab.
         * </returns>
         *
         * <param name="a">The first point on the line.</param>
         * <param name="b">The second point on the line.</param>
         * <param name="c">The point to which the signed distance is to be
         * calculated.</param>
         */
        internal static FFloat leftOf(FVector3 a, FVector3 b, FVector3 c)
        {
            return det(a - c, b - a);
        }

        /**
         * <summary>Computes the square of a float.</summary>
         *
         * <returns>The square of the float.</returns>
         *
         * <param name="scalar">The float to be squared.</param>
         */
        internal static FFloat sqr(FFloat scalar)
        {
            return scalar * scalar;
        }

        /**
         * <summary>Computes the square root of a float.</summary>
         *
         * <returns>The square root of the float.</returns>
         *
         * <param name="scalar">The float of which to compute the square root.
         * </param>
         */
        internal static FFloat sqrt(FFloat scalar)
        {
#if USE_FIXEDMATH
            return FMath.Sqrt(scalar);
#else
            return (FFloat)Math.Sqrt((double)scalar);
#endif
        }
        public static FVector3 ProjectPointToSegment(FVector3 p, FVector3 a, FVector3 b)
        {
            FVector3 ab = b - a;
            float t = FVector3.Dot(p - a, ab) / ab.sqrMagnitude;
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            return a + ab * t;
        }
    }
}
