﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedParameter.Local

/* Utility.cs -- сборник простых вспомогательных методов.
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

#nullable enable

namespace AM
{
    /// <summary>
    /// Сборник простых вспомогательных методов.
    /// </summary>
    public static class Utility
    {
        #region Properties

        /// <summary>
        /// Gets the CP866 (cyrillic) <see cref="Encoding"/>.
        /// </summary>
        public static Encoding Cp866
        {
            [DebuggerStepThrough]
            get
            {
                if (ReferenceEquals(_cp866, null))
                {
                    RegisterEncodingProviders();
                    _cp866 = Encoding.GetEncoding(866);
                }

                return _cp866;
            }
        }

        /// <summary>
        /// Gets the Windows-1251 (cyrillic) <see cref="Encoding"/>.
        /// </summary>
        public static Encoding Windows1251
        {
            [DebuggerStepThrough]
            get
            {
                if (ReferenceEquals(_windows1251, null))
                {
                    RegisterEncodingProviders();
                    _windows1251 = Encoding.GetEncoding(1251);
                }

                return _windows1251;
            }
        }

        #endregion

        #region Private members

        private static Encoding? _cp866, _windows1251;

        #endregion

        #region Public methods

        /// <summary>
        /// Is digit from 0 to 9?
        /// </summary>
        public static bool IsArabicDigit
            (
                this char c
            )
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Is letter from A to Z or a to z?
        /// </summary>
        public static bool IsLatinLetter
            (
                this char c
            )
        {
            return c >= 'A' && c <= 'Z'
                   || c >= 'a' && c <= 'z';
        }

        /// <summary>
        /// Is digit from 0 to 9
        /// or letter from A to Z or a to z?
        /// </summary>
        public static bool IsLatinLetterOrArabicDigit
            (
                this char c
            )
        {
            return c >= '0' && c <= '9'
                   || c >= 'A' && c <= 'Z'
                   || c >= 'a' && c <= 'z';
        }

        /// <summary>
        /// Is letter from А to Я or а to я?
        /// </summary>
        public static bool IsRussianLetter
            (
                this char c
            )
        {
            return c >= 'А' && c <= 'я'
                   || c == 'Ё' || c == 'ё';
        }

        /// <summary>
        /// Перенаправление стандартного вывода в файл.
        /// </summary>
        public static void RedirectStandardOutput
            (
                string fileName,
                Encoding encoding
            )
        {
            //Code.NotNullNorEmpty(fileName, "fileName");
            //Code.NotNull(encoding, "encoding");

            var stdOutput = new StreamWriter
                (
                    new FileStream
                    (
                        fileName,
                        FileMode.Create
                    ),
                    encoding
                )
            {
                AutoFlush = true
            };

            Console.SetOut(stdOutput);
        }

        /// <summary>
        /// Переключение кодовой страницы вывода консоли.
        /// </summary>
        public static void SetOutputCodePage
            (
                Encoding encoding
            )
        {
            // Code.NotNull(encoding, "encoding");

            StreamWriter stdOutput = new StreamWriter
            (
                Console.OpenStandardOutput(),
                encoding
            )
            {
                AutoFlush = true
            };
            Console.SetOut(stdOutput);

            StreamWriter stdError = new StreamWriter
            (
                Console.OpenStandardError(),
                encoding
            )
            {
                AutoFlush = true
            };
            Console.SetError(stdError);
        }

        /// <summary>
        /// Переключение кодовой страницы вывода консоли.
        /// </summary>
        public static void SetOutputCodePage
            (
                int codePage
            )
        {
            SetOutputCodePage
            (
                Encoding.GetEncoding(codePage)
            );
        }

        /// <summary>
        /// Detect AppVeyor CI environment.
        /// </summary>
        public static bool DetectAppVeyor()
        {
            return Environment.GetEnvironmentVariable("APPVEYOR") == "True";
        }

        /// <summary>
        /// Detect generic CI environment.
        /// </summary>
        public static bool DetectCI()
        {
            return Environment.GetEnvironmentVariable("CI") == "True";
        }

        /// <summary>
        /// Detect generic Travis CI environment.
        /// </summary>
        public static bool DetectTravis()
        {
            return Environment.GetEnvironmentVariable("TRAVIS") == "True";
        }

        /// <summary>
        /// Бросает исключение, если переданное значение равно <c>null</c>,
        /// иначе просто возвращает его.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        public static T ThrowIfNull<T>
            (
                this T? value,
                string message
            )
            where T : class
        {
            //Sure.NotNull(message, nameof(message));

            if (ReferenceEquals(value, null))
            {
                //Log.Error
                //(
                //    nameof(Utility) + "::" + nameof(ThrowIfNull)
                //    + ": "
                //    + message
                //);

                throw new ArgumentException (message);
            }

            return value;
        }

        /// <summary>
        /// Бросает исключение, если переданное значение равно <c>null</c>,
        /// иначе просто возвращает его.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        [DebuggerStepThrough]
        public static T ThrowIfNull<T>
            (
                this T? value
            )
            where T : class
        {
            return ThrowIfNull<T>
                (
                    value,
                    "Null value detected"
                );
        }

        /// <summary>
        /// Превращает объект в видимую строку.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public static string ToVisibleString<T>
            (
                this T? value
            )
            where T : class
        {
            var result = value?.ToString();

            if (ReferenceEquals(result, null))
            {
                return "(null)";
            }

            return result;
        }

        /// <summary>
        /// Сравнивает две строки независимо от текущей культуры.
        /// </summary>
        public static bool EqualInvariant
            (
                string? left,
                string? right
            )
        {
            return CultureInfo.InvariantCulture.CompareInfo.Compare
                (
                    left,
                    right
                ) == 0;
        }

        /// <summary>
        /// Gets the first char of the text.
        /// </summary>
        public static char FirstChar
            (
                this string? text
            )
        {
            return string.IsNullOrEmpty(text)
                ? '\0'
                : text[0];
        }

        /// <summary>
        /// Gets the last char of the text.
        /// </summary>
        public static char LastChar
            (
                this string? text
            )
        {
            return string.IsNullOrEmpty(text)
                ? '\0'
                : text[^1];
        }

        /// <summary>
        /// Сравнивает символы с точностью до регистра.
        /// </summary>
        /// <param name="one">Первый символ.</param>
        /// <param name="two">Второй символ.</param>
        /// <returns>Символы совпадают с точностью до регистра.</returns>
        public static bool SameChar
            (
                this char one,
                char two
            )
        {
            return char.ToUpperInvariant(one) == char.ToUpperInvariant(two);
        }

        /// <summary>
        /// Сравнивает строки с точностью до регистра.
        /// </summary>
        /// <param name="one">Первая строка.</param>
        /// <param name="two">Вторая строка.</param>
        /// <returns>Строки совпадают с точностью до регистра.</returns>
        public static bool SameString
            (
                this string? one,
                string? two
            )
        {
            return string.Compare
                (
                    one,
                    two,
                    StringComparison.OrdinalIgnoreCase
                ) == 0;
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this short value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this ushort value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this int value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this uint value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this long value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this ulong value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Register required encoding providers.
        /// </summary>
        public static void RegisterEncodingProviders()
        {
            Encoding.RegisterProvider
                (
                    CodePagesEncodingProvider.Instance
                );
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static int SafeToInt32
            (
                this string? text,
                int defaultValue,
                int minValue,
                int maxValue
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            if (!int.TryParse(text, out var result))
            {
                result = defaultValue;
            }

            if (result < minValue
                || result > maxValue)
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static int SafeToInt32
            (
                this string? text,
                int defaultValue
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }

            if (!int.TryParse(text, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Безопасное преобразование строки в целое.
        /// </summary>
        public static int SafeToInt32
            (
                this string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            if (!int.TryParse(text, out var result))
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Determines whether is one of the specified values.
        /// </summary>
        [Pure]
        public static bool IsOneOf<T>
            (
                this T value,
                params T[] array
            )
            where T : IComparable<T>
        {
            foreach (var one in array)
            {
                if (value.CompareTo(one) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts empty string to <c>null</c>.
        /// </summary>
        public static string? EmptyToNull
            (
                this string? value
            )
        {
            return string.IsNullOrEmpty(value)
                ? null
                : value;
        }

        #endregion
    }
}
