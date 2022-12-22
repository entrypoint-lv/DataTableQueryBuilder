import{_ as e,c as t,o as l,a}from"./app.9dbf2506.js";const u=JSON.parse('{"title":"Individual Field Options","description":"","frontmatter":{},"headers":[{"level":2,"title":"UseMatchMode","slug":"usematchmode","link":"#usematchmode","children":[]},{"level":2,"title":"UseSourceProperty","slug":"usesourceproperty","link":"#usesourceproperty","children":[]},{"level":2,"title":"SearchBy","slug":"searchby","link":"#searchby","children":[]},{"level":2,"title":"OrderBy","slug":"orderby","link":"#orderby","children":[]},{"level":2,"title":"EnableGlobalSearch","slug":"enableglobalsearch","link":"#enableglobalsearch","children":[]}],"relativePath":"guide/configuration/field-options.md"}'),r={name:"guide/configuration/field-options.md"},i=a('<h1 id="individual-field-options" tabindex="-1">Individual Field Options <a class="header-anchor" href="#individual-field-options" aria-hidden="true">#</a></h1><h2 id="usematchmode" tabindex="-1">UseMatchMode <a class="header-anchor" href="#usematchmode" aria-hidden="true">#</a></h2><p>A method to explicitly set the value matching strategy to be used when filtering. Applicable only to properties of type <code>String</code> or integer numeric types.</p><ul><li>Arguments:</li></ul><table><thead><tr><th style="text-align:left;">Name</th><th style="text-align:left;">Type</th><th style="text-align:left;">Comment</th></tr></thead><tbody><tr><td style="text-align:left;">mode</td><td style="text-align:left;">Enum of type <code>StringMatchMode</code> or <code>IntegerMatchMode</code></td><td style="text-align:left;">-</td></tr></tbody></table><h2 id="usesourceproperty" tabindex="-1">UseSourceProperty <a class="header-anchor" href="#usesourceproperty" aria-hidden="true">#</a></h2><p>A method to explicitly set the property to be used when filtering and sorting.</p><ul><li>Arguments:</li></ul><table><thead><tr><th style="text-align:left;">Name</th><th style="text-align:left;">Type</th><th style="text-align:left;">Comment</th></tr></thead><tbody><tr><td style="text-align:left;">property</td><td style="text-align:left;"><code>Expression&lt;Func&lt;T, TMember&gt;&gt;</code></td><td style="text-align:left;">Expression to the property</td></tr></tbody></table><h2 id="searchby" tabindex="-1">SearchBy <a class="header-anchor" href="#searchby" aria-hidden="true">#</a></h2><p>A method to explicitly set the search expression to be used when filtering.</p><ul><li>Arguments:</li></ul><table><thead><tr><th style="text-align:left;">Name</th><th style="text-align:left;">Type</th><th style="text-align:left;">Comment</th></tr></thead><tbody><tr><td style="text-align:left;">expression</td><td style="text-align:left;"><code>Expression&lt;Func&lt;T, string, bool&gt;&gt;</code></td><td style="text-align:left;">Search expression</td></tr></tbody></table><h2 id="orderby" tabindex="-1">OrderBy <a class="header-anchor" href="#orderby" aria-hidden="true">#</a></h2><p>A method to explicitly set the sort expression to be used when sorting.</p><ul><li>Arguments:</li></ul><table><thead><tr><th style="text-align:left;">Name</th><th style="text-align:left;">Type</th><th style="text-align:left;">Comment</th></tr></thead><tbody><tr><td style="text-align:left;">expression</td><td style="text-align:left;"><code>Expression&lt;Func&lt;T, object&gt;&gt;</code></td><td style="text-align:left;">Sort expression</td></tr></tbody></table><h2 id="enableglobalsearch" tabindex="-1">EnableGlobalSearch <a class="header-anchor" href="#enableglobalsearch" aria-hidden="true">#</a></h2><p>A method to enable a global search on this field. Applicable only to JavaScript datatables supporting global search option.</p>',19),o=[i];function d(s,n,h,c,p,y){return l(),t("div",null,o)}const b=e(r,[["render",d]]);export{u as __pageData,b as default};