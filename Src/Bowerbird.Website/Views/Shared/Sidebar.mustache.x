﻿<div id="action-menu" class="tab-bar small">
  <ul class="tabs">
    <li><a href="/observations/create" class="tab-button new-observation-button">New Observation <span></span></a></li>
    <li><a href="/projects/create" class="tab-button new-project-button">New Project <span></span></a></li>
    <!--<li><a href="/teams/create" class="tab-image-button browse-projects-button">New Team <span></span></a></li>
    <li><a href="/organisations/create" class="tab-image-button browse-projects-button">New Organisation <span></span></a></li>-->
    
  </ul>
</div>  
<div class="menu-group" id="default-menu-group">
  <ul>
      <li class="menu-group-item"><a href="/" class="user-stream"><img alt="" src="../img/avatar-1.png"><div><p>Home</p></div></a></li>
      <li class="menu-group-item"><a href="/{{Model.User.Id}}" class="user-profile"><img alt="" src="../img/avatar-1.png"><div><p>Your Profile</p></div></a></li>
      <li class="menu-group-item"><a href="/all" class="all-bowerbird"><img alt="" src="../img/avatar-1.png"><div><p>All Bowerbird Activity</p></div></a></li>
      <li class="menu-group-item"><a href="/favourites" class="user-favourites"><img alt="" src="../img/avatar-2.png"><div><p>Favourites</p></div></a></li>
  </ul>
</div>
<div class="menu-projects"></div>
<div class="menu-teams"></div>
<div class="menu-organisations"></div>