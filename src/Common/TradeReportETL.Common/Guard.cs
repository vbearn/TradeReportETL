#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TradeReportETL.Common
{
    public static class Guard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(
            [NotNull] T? value, string argumentName,
            [CallerMemberName] string memberName = null!) where T : class
        {
            if (value is null)
            {
                // .ldstr prevents inlining, thus a dedicated method which throws with the proper message.
                ThrowBecauseArgumentIsNull(argumentName, memberName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(
            [NotNull] T? value, string argumentName, string customMessage,
            [CallerMemberName] string memberName = null!) where T : class
        {
            if (value is null)
            {
                // .ldstr prevents inlining, thus a dedicated method which throws with the proper message.
                ThrowBecauseArgumentIsNull(argumentName, memberName, customMessage);
            }
        }

        /// <summary>
        /// Checks value is not <c>null</c>. Otherwise throws <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">Type of the instance.</typeparam>
        /// <param name="value">Value checked against null.</param>
        /// <param name="argumentName">
        /// Name of the checked argument.
        /// NB: When Caller argument expression is finally supported
        /// (https://github.com/dotnet/csharplang/blob/master/proposals/caller-argument-expression.md)
        /// this parameter won't be required anymore.</param>
        /// <param name="memberName">Name of the member performing the check. (.ctor in most cases).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public static T ProvidedNotNull<T>(
            this T? value, string argumentName,
            [CallerMemberName] string memberName = null!) where T : class
        {
            NotNull(value, argumentName, memberName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>(
            [NotNull] T? value, string argumentName, [CallerMemberName] string memberName = null!) where T : struct
        {
            if (value is null)
            {
                ThrowBecauseArgumentIsNull(argumentName, memberName);
            }
        }

        /// <summary>
        /// Checks value is not <c>null</c>. Otherwise throws <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">Type of the instance.</typeparam>
        /// <param name="value">Value checked against null.</param>
        /// <param name="argumentName">
        /// Name of the checked argument.
        /// NB: When Caller argument expression is finally supported
        /// (https://github.com/dotnet/csharplang/blob/master/proposals/caller-argument-expression.md)
        /// this parameter won't be required anymore.</param>
        /// <param name="memberName">Name of the member performing the check. (.ctor in most cases).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNull]
        public static T ProvidedNotNull<T>(
            this T? value, string argumentName,
            [CallerMemberName] string memberName = null!) where T : struct
        {
            NotNull(value, argumentName, memberName);

            return value.Value;
        }

        /// <exception cref="ArgumentNullException"> thrown if <paramref name="value"/> or <paramref name="predicate"/> is null</exception>
        public static TMember ProvidedNotNull<TRoot, TMember>(this TRoot? value, Func<TRoot, TMember> predicate, string argumentName, [CallerMemberName] string memberName = null!)
            where TRoot : class
            where TMember : class
        {
            var rootObject = value.ProvidedNotNull(argumentName, memberName);

            return predicate.Invoke(rootObject).ProvidedNotNull(argumentName, memberName);
        }

        [DoesNotReturn]
        private static void ThrowBecauseArgumentIsNull(
            string argumentName, string memberName)
        {
            throw new ArgumentNullException(
                argumentName, $"'{argumentName}' is null in '{memberName}'.");
        }

        [DoesNotReturn]
        private static void ThrowBecauseArgumentIsNull(string argumentName, string memberName, string customMessage)
        {
            throw new ArgumentNullException(argumentName, $"'{argumentName}' is null in '{memberName}'. {customMessage}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotWhitespaceString(
            [NotNull] string? value, string argumentName, [CallerMemberName] string memberName = null!)
        {
            if (value is null)
            {
                ThrowBecauseArgumentIsNull(argumentName, memberName);
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                ThrowBecauseArgumentIsWhitespaceString(argumentName, memberName);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ProvidedNotWhitespaceString(
            this string value, string argumentName, [CallerMemberName] string memberName = null!)
        {
            NotWhitespaceString(value, argumentName, memberName);

            return value;
        }

        [DoesNotReturn]
        private static void ThrowBecauseArgumentIsWhitespaceString(
            string argumentName, string memberName)
        {
            throw new ArgumentException(
                $"'{argumentName}' is a whitespace string in '{memberName}'.", argumentName);
        }

        public static void NotZero(
          decimal value, string argumentName, [CallerMemberName] string memberName = null!)
        {
            if (value == 0)
            {
                ThrowBecauseArgumentIsLessThanZero(argumentName, memberName);
            }
        }

        public static void NotNegative(
            decimal value, string argumentName, [CallerMemberName] string memberName = null!)
        {
            if (value < 0)
            {
                ThrowBecauseArgumentIsLessThanZero(argumentName, memberName);
            }
        }

        public static void NotNegative(
            float value, string argumentName, [CallerMemberName] string memberName = null!)
        {
            if (value < 0)
            {
                ThrowBecauseArgumentIsLessThanZero(argumentName, memberName);
            }
        }

        public static void NotNegative(
            int value, string argumentName, [CallerMemberName] string memberName = null!)
        {
            if (value < 0)
            {
                ThrowBecauseArgumentIsLessThanZero(argumentName, memberName);
            }
        }

        [DoesNotReturn]
        private static void ThrowBecauseArgumentIsLessThanZero(
            string argumentName, string memberName)
        {
            throw new ArgumentException(
                $"'{argumentName}' cannot be negative in '{memberName}'.", argumentName);
        }
    }
}
