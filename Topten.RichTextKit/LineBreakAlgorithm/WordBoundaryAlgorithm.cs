﻿// RichTextKit
// Copyright © 2019-2020 Topten Software. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may 
// not use this product except in compliance with the License. You may obtain 
// a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations 
// under the License.
//
// Ported from: https://github.com/foliojs/linebreak

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Topten.RichTextKit.Utils;

namespace Topten.RichTextKit
{
    /// <summary>
    /// Implementation of the word boundary algorithm
    /// </summary>
    internal class WordBoundaryAlgorithm
    {
        /// <summary>
        /// Locate the start of each "word" in a unicode string.  Used for Ctrl+Left/Right
        /// in editor and different to the line break algorithm.
        /// </summary>
        public static IEnumerable<int> FindWordBoundaries(Slice<int> codePoints)
        {
            // Start is always a word boundary
            yield return 0;

            // Find all boundaries
            bool inWord = false;
            var wordGroup = BoundaryGroup.Ignore;
            for (int i=0; i<codePoints.Length; i++)
            {
                // Get group
                var bg = UnicodeClasses.BoundaryGroup(codePoints[i]);

                // Ignore?
                if (bg == BoundaryGroup.Ignore)
                    continue;

                // Ignore spaces before word
                if (!inWord)
                {
                    // Ignore spaces before word
                    if (bg == BoundaryGroup.Space)
                        continue;

                    // Found start of word
                    if (i != 0)
                        yield return i;

                    // We're now in the word
                    inWord = true;
                    wordGroup = bg;
                    continue;
                }

                // We're in a word group, check for change of kind
                if (wordGroup != bg)
                {
                    if (bg == BoundaryGroup.Space)
                    {
                        inWord = false;
                    }
                    else
                    {
                        // Switch to a different word kind without a space
                        // just emit a word boundary here
                        yield return i;
                    }
                }
            }

            if (!inWord && codePoints.Length > 0)
            {
                yield return codePoints.Length;
            }
        }
    }
}
