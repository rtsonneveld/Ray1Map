﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine
{
    public class SpyroEncoding : Encoding
    {
        public SpyroEncoding(GameModeSelection gameModeSelection)
        {
            GameModeSelection = gameModeSelection;
            CharTable = GetCharTable(gameModeSelection);
        }

        public GameModeSelection GameModeSelection { get; }
        public Dictionary<byte, char> CharTable { get; }

        public override int GetByteCount(char[] chars, int index, int count) => count;

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => throw new System.NotImplementedException();

        public override int GetCharCount(byte[] bytes, int index, int count) => bytes.Skip(index).Take(count).Count(x => x != 0xFF);

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = byteIndex; i < byteCount; i++)
            {
                if (bytes[i] == 0xFF)
                    continue;

                chars[charIndex] = CharTable.TryGetItem(bytes[i]);
                charIndex++;
            }

            return byteCount;
        }

        public Dictionary<byte, char> GetCharTable(GameModeSelection gameMode)
        {
            switch (gameMode)
            {
                case GameModeSelection.SpyroSeasonFlameEU:
                    return new Dictionary<byte, char>()
                    {
                        [0] = ' ',
                        [1] = '!',
                        [2] = '"',
                        [3] = '%',
                        [4] = '\'',
                        [5] = '(',
                        [6] = ')',
                        [7] = ',',
                        [8] = '-',
                        [9] = '.',
                        [10] = '/',
                        [11] = '0',
                        [12] = '1',
                        [13] = '2',
                        [14] = '3',
                        [15] = '4',
                        [16] = '5',
                        [17] = '6',
                        [18] = '7',
                        [19] = '8',
                        [20] = '9',

                        [21] = ':',
                        [22] = ';',
                        [23] = '?',

                        [24] = 'A',
                        [25] = 'B',
                        [26] = 'C',
                        [27] = 'D',
                        [28] = 'E',
                        [29] = 'F',
                        [30] = 'G',
                        [31] = 'H',
                        [32] = 'I',
                        [33] = 'J',
                        [34] = 'K',
                        [35] = 'L',
                        [36] = 'M',
                        [37] = 'N',
                        [38] = 'O',
                        [39] = 'P',
                        [40] = 'Q',
                        [41] = 'R',
                        [42] = 'S',
                        [43] = 'T',
                        [44] = 'U',
                        [45] = 'V',
                        [46] = 'W',
                        [47] = 'X',
                        [48] = 'Y',
                        [49] = 'Z',

                        [50] = 'a',
                        [51] = 'b',
                        [52] = 'c',
                        [53] = 'd',
                        [54] = 'e',
                        [55] = 'f',
                        [56] = 'g',
                        [57] = 'h',
                        [58] = 'i',
                        [59] = 'j',
                        [60] = 'k',
                        [61] = 'l',
                        [62] = 'm',
                        [63] = 'n',
                        [64] = 'o',
                        [65] = 'p',
                        [66] = 'q',
                        [67] = 'r',
                        [68] = 's',
                        [69] = 't',
                        [70] = 'u',
                        [71] = 'v',
                        [72] = 'w',
                        [73] = 'x',
                        [74] = 'y',
                        [75] = 'z',

                        [76] = ' ',
                        [77] = '¡',
                        [78] = '<',
                        [79] = '>',
                        [80] = '¿',

                        [81] = 'Á',
                        [82] = 'Ä',
                        [83] = 'Ç',
                        [84] = 'È',
                        [85] = 'É',
                        [86] = 'Í',
                        [87] = 'Ñ',
                        [88] = 'Ó',
                        [89] = 'Ö',
                        [90] = 'Ü',
                        [91] = 'ß',

                        [92] = 'à',
                        [93] = 'á',
                        [94] = 'â',
                        [95] = 'ä',
                        [96] = 'ç',
                        [97] = 'è',
                        [98] = 'é',
                        [99] = 'ê',
                        [100] = 'ì',
                        [101] = 'í',
                        [102] = 'î',
                        [103] = 'ï',
                        [104] = 'ñ',
                        [105] = 'ò',
                        [106] = 'ó',
                        [107] = 'ô',
                        [108] = 'ö',
                        [109] = 'ù',
                        [110] = 'ú',
                        [111] = 'û',
                        [112] = 'ü',
                    };

                case GameModeSelection.SpyroSeasonFlameUS:
                    return new Dictionary<byte, char>()
                    {
                        [0] = ' ',
                        [1] = '!',
                        [2] = '"',
                        [3] = '%',
                        [4] = '\'',
                        [5] = '(',
                        [6] = ')',
                        [7] = ',',
                        [8] = '-',
                        [9] = '.',
                        [10] = '/',
                        [11] = '0',
                        [12] = '1',
                        [13] = '2',
                        [14] = '3',
                        [15] = '4',
                        [16] = '5',
                        [17] = '6',
                        [18] = '7',
                        [19] = '8',
                        [20] = '9',

                        [21] = ':',
                        [22] = ';',
                        [23] = '?',

                        [24] = 'A',
                        [25] = 'B',
                        [26] = 'C',
                        [27] = 'D',
                        [28] = 'E',
                        [29] = 'F',
                        [30] = 'G',
                        [31] = 'H',
                        [32] = 'I',
                        [33] = 'J',
                        [34] = 'K',
                        [35] = 'L',
                        [36] = 'M',
                        [37] = 'N',
                        [38] = 'O',
                        [39] = 'P',
                        [40] = 'Q',
                        [41] = 'R',
                        [42] = 'S',
                        [43] = 'T',
                        [44] = 'U',
                        [45] = 'V',
                        [46] = 'W',
                        [47] = 'X',
                        [48] = 'Y',
                        [49] = 'Z',

                        [50] = 'a',
                        [51] = 'b',
                        [52] = 'c',
                        [53] = 'd',
                        [54] = 'e',
                        [55] = 'f',
                        [56] = 'g',
                        [57] = 'h',
                        [58] = 'i',
                        [59] = 'j',
                        [60] = 'k',
                        [61] = 'l',
                        [62] = 'm',
                        [63] = 'n',
                        [64] = 'o',
                        [65] = 'p',
                        [66] = 'q',
                        [67] = 'r',
                        [68] = 's',
                        [69] = 't',
                        [70] = 'u',
                        [71] = 'v',
                        [72] = 'w',
                        [73] = 'x',
                        [74] = 'y',
                        [75] = 'z',

                        [97] = 'é', // TODO: Is this correct?
                    };

                case GameModeSelection.SpyroAdventureEU:
                    return new Dictionary<byte, char>()
                    {
                        [0] = ' ',
                        [1] = '!',
                        [2] = '"',
                        [3] = '#',
                        [4] = '%',
                        [5] = '&',
                        [6] = '\'',
                        [7] = '[',
                        [8] = ']',
                        [9] = '*',
                        [10] = '+',
                        [11] = ',',
                        [12] = '-',
                        [13] = '.',
                        [14] = '/',
                        [15] = '0',
                        [16] = '1',
                        [17] = '2',
                        [18] = '3',
                        [19] = '4',
                        [20] = '5',
                        [21] = '6',
                        [22] = '7',
                        [23] = '8',
                        [24] = '9',

                        [25] = ':',
                        [26] = ';',
                        [27] = '?',

                        [28] = 'A',
                        [29] = 'B',
                        [30] = 'C',
                        [31] = 'D',
                        [32] = 'E',
                        [33] = 'F',
                        [34] = 'G',
                        [35] = 'H',
                        [36] = 'I',
                        [37] = 'J',
                        [38] = 'K',
                        [39] = 'L',
                        [40] = 'M',
                        [41] = 'N',
                        [42] = 'O',
                        [43] = 'P',
                        [44] = 'Q',
                        [45] = 'R',
                        [46] = 'S',
                        [47] = 'T',
                        [48] = 'U',
                        [49] = 'V',
                        [50] = 'W',
                        [51] = 'X',
                        [52] = 'Y',
                        [53] = 'Z',

                        [54] = 'a',
                        [55] = 'b',
                        [56] = 'c',
                        [57] = 'd',
                        [58] = 'e',
                        [59] = 'f',
                        [60] = 'g',
                        [61] = 'h',
                        [62] = 'i',
                        [63] = 'j',
                        [64] = 'k',
                        [65] = 'l',
                        [66] = 'm',
                        [67] = 'n',
                        [68] = 'o',
                        [69] = 'p',
                        [70] = 'q',
                        [71] = 'r',
                        [72] = 's',
                        [73] = 't',
                        [74] = 'u',
                        [75] = 'v',
                        [76] = 'w',
                        [77] = 'x',
                        [78] = 'y',
                        [79] = 'z',

                        [80] = '¡',
                        [81] = 'º',
                        [82] = '¿',

                        [83] = 'Á',
                        [84] = 'Ä',
                        [85] = 'Ç',
                        [86] = 'È',
                        [87] = 'É',
                        [88] = 'Ì',
                        [89] = 'Í',
                        [90] = 'Ñ',
                        [91] = 'Ó',
                        [92] = 'Ù',
                        [93] = 'Ú',
                        [94] = 'Ü',
                        [95] = 'ß',

                        [96] = 'à',
                        [97] = 'á',
                        [98] = 'â',
                        [99] = 'ä',
                        [100] = 'ç',
                        [101] = 'è',
                        [102] = 'é',
                        [103] = 'ê',
                        [104] = 'ë',
                        [105] = 'ì',
                        [106] = 'í',
                        [107] = 'î',
                        [108] = 'ï',
                        [109] = 'ñ',
                        [110] = 'ò',
                        [111] = 'ó',
                        [112] = 'ô',
                        [113] = 'ö',
                        [114] = 'ù',
                        [115] = 'ú',
                        [116] = 'û',
                        [117] = 'ü',

                        [118] = '™',

                        [254] = '\n',
                        //[255] = (char)0xAD,
                    };

                case GameModeSelection.SpyroAdventureUS:
                    return new Dictionary<byte, char>()
                    {
                        [0] = ' ',
                        [1] = '!',
                        [2] = '#',
                        [3] = '%',
                        [4] = '\'',
                        [5] = '[',
                        [6] = ']',
                        [7] = '*',
                        [8] = '+',
                        [9] = ',',
                        [10] = '-',
                        [11] = '.',
                        [12] = '/',
                        [13] = '0',
                        [14] = '1',
                        [15] = '2',
                        [16] = '3',
                        [17] = '4',
                        [18] = '5',
                        [19] = '6',
                        [20] = '7',
                        [21] = '8',
                        [22] = '9',

                        [23] = ':',
                        [24] = '?',

                        [25] = 'A',
                        [26] = 'B',
                        [27] = 'C',
                        [28] = 'D',
                        [29] = 'E',
                        [30] = 'F',
                        [31] = 'G',
                        [32] = 'H',
                        [33] = 'I',
                        [34] = 'J',
                        [35] = 'K',
                        [36] = 'L',
                        [37] = 'M',
                        [38] = 'N',
                        [39] = 'O',
                        [40] = 'P',
                        [41] = 'Q',
                        [42] = 'R',
                        [43] = 'S',
                        [44] = 'T',
                        [45] = 'U',
                        [46] = 'V',
                        [47] = 'W',
                        [48] = 'X',
                        [49] = 'Y',
                        [50] = 'Z',

                        [51] = 'a',
                        [52] = 'b',
                        [53] = 'c',
                        [54] = 'd',
                        [55] = 'e',
                        [56] = 'f',
                        [57] = 'g',
                        [58] = 'h',
                        [59] = 'i',
                        [60] = 'j',
                        [61] = 'k',
                        [62] = 'l',
                        [63] = 'm',
                        [64] = 'n',
                        [65] = 'o',
                        [66] = 'p',
                        [67] = 'q',
                        [68] = 'r',
                        [69] = 's',
                        [70] = 't',
                        [71] = 'u',
                        [72] = 'v',
                        [73] = 'w',
                        [74] = 'x',
                        [75] = 'y',
                        [76] = 'z',

                        [77] = '™',
                        [78] = 'Ç',
                        [79] = 'Ñ',

                        //[255] = (char)0xAD,
                    };

                default:
                    return new Dictionary<byte, char>();
            }
        }

        public override int GetMaxByteCount(int charCount) => charCount;

        public override int GetMaxCharCount(int byteCount) => byteCount;
    }
}