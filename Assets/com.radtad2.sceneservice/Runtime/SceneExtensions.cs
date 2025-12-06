using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;

namespace SceneService
{
    public static class SceneExtensions
    {
        /// <summary>
        /// Converts a ReloadExistingScenesPolicy enum value into individual flags for which categories to reload.
        /// </summary>
        /// <param name="policy">The policy to convert.</param>
        /// <param name="reloadActive">Outputs whether the active scene should be reloaded.</param>
        /// <param name="reloadDependencies">Outputs whether dependencies should be reloaded.</param>
        /// <param name="reloadExtras">Outputs whether runtime extras should be reloaded.</param>
        public static void Decompose(
            this ReloadPolicy policy,
            out bool reloadActive,
            out bool reloadDependencies,
            out bool reloadExtras)
        {
            reloadActive = false;
            reloadDependencies = false;
            reloadExtras = false;

            switch (policy)
            {
                case ReloadPolicy.None:
                    break;

                case ReloadPolicy.Active:
                    reloadActive = true;
                    break;

                case ReloadPolicy.Dependencies:
                    reloadDependencies = true;
                    break;

                case ReloadPolicy.Extras:
                    reloadExtras = true;
                    break;

                case ReloadPolicy.ActiveAndDependencies:
                    reloadActive = true;
                    reloadDependencies = true;
                    break;

                case ReloadPolicy.ActiveAndExtras:
                    reloadActive = true;
                    reloadExtras = true;
                    break;

                case ReloadPolicy.DependenciesAndExtras:
                    reloadDependencies = true;
                    reloadExtras = true;
                    break;

                case ReloadPolicy.All:
                    reloadActive = true;
                    reloadDependencies = true;
                    reloadExtras = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(policy), policy, $"Unknown {nameof(ReloadPolicy)} value");
            }
        }

        /// <summary>
        /// Tells if a scene reference is safe and loaded.
        /// </summary>
        /// <param name="reference">The scene</param>
        /// <returns>True if safe and loaded.</returns>
        public static bool IsSafeAndLoaded(this SceneReference reference)
        {
            return reference.UnsafeReason is SceneReferenceUnsafeReason.None &&
                   reference.LoadedScene.isLoaded;
        }

        /// <summary>
        /// Tells if the list contains the reference by checking if any element shares its guid.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if it exists in the list.</returns>
        public static bool ContainsByGuid(this List<SceneReference> list, SceneReference element)
        {
            return list.Any(r => r.Guid == element.Guid);
        }
        
        /// <summary>
        /// Removes an element from a list by its guid.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="element">The element.</param>
        public static void RemoveByGuid(this List<SceneReference> list, SceneReference element)
        {
            list.RemoveAll(r => r.Guid == element.Guid);
        }
    }
}