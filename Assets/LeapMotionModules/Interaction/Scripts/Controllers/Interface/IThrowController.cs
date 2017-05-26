/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using Leap.Unity.Interaction.Internal;
using UnityEngine;

namespace Leap.Unity.Interaction {

  /// <summary>
  /// Interaction Behaviours feed their throw controllers callbacks when they are
  /// held (for data collection) and when they are released so that the controller
  /// can manipulate the Interaction object's thrown direction and velocity to match
  /// the user's intention.
  /// </summary>
  public interface IThrowController {

    /// <summary>
    /// Called every FixedUpdate frame while an interaction object is being held.
    /// </summary>
    /// <param name="intObj">The interaction object being held.</param>
    /// <param name="hands">A list of the interaction controllers currently grasping
    /// the object.</param>
    void OnHold(InteractionBehaviour intObj,
               ReadonlyList<InteractionController> controllers);

    /// <summary>
    /// Called when an Interaction object is released by the last interaction controller
    /// holding it.
    /// </summary>
    void OnThrow(InteractionBehaviour intObj, InteractionController controller);

  }

}
