using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapMinimap
{
    public class FogRenderer : MonoBehaviour
    {
        [Header("Fog")]
        public Material bg_material;
        public Material fog_material;
        public Color fog_color = Color.black;
        public int render_texture_size = 1024;
        public bool multi_resolution = true;

        private MapViewer viewer;
        private MapZone zone;
        private Material fog_mat;
        private Material bg_mat;
        private Vector2Int texture_size;
        private float aspect_ratio = 1f;
        private bool inited = false;
        private bool rendered = false;

        private Dictionary<int, RenderTexture> render_textures = new Dictionary<int, RenderTexture>();

        private void Awake()
        {
            viewer = GetComponentInParent<MapViewer>();
            texture_size = new Vector2Int(render_texture_size, render_texture_size);
        }

        private void Start()
        {
            if (!inited)
                InitRenderer();
        }

        private void OnDestroy()
        {
            foreach (RenderTexture texture in render_textures.Values)
                texture.Release();
            render_textures.Clear();
        }

        private void InitRenderer()
        {
            fog_mat = new Material(fog_material);
            fog_mat.SetColor("_Color", fog_color);
            bg_mat = new Material(bg_material);
            bg_mat.SetColor("_Color", fog_color);
            inited = true;
        }

        private RenderTexture InitZone(MapZone zone)
        {
            if (zone != null && !render_textures.ContainsKey(zone.zone_id))
            {
                RenderTexture render_texture = new RenderTexture(texture_size.x, texture_size.y, 0, RenderTextureFormat.ARGB32);
                render_textures[zone.zone_id] = render_texture;
                return render_texture;
            }
            return null;
        }

        public void InitAndRender()
        {
            if (!inited)
                InitRenderer();

            if (inited)
                RenderFog();
        }

        public void SetZone(MapZone zone)
        {
            if (zone != null && zone != this.zone && zone.map != null)
            {
                this.zone = zone;
                aspect_ratio = zone.map.width / (float)zone.map.height;
                if (aspect_ratio > 1f)
                    texture_size = new Vector2Int(render_texture_size, Mathf.RoundToInt(render_texture_size / aspect_ratio));
                else
                    texture_size = new Vector2Int(Mathf.RoundToInt(render_texture_size * aspect_ratio), render_texture_size);

                rendered = false;
                InitZone(zone);
            }
        }

        public void RenderFog()
        {
            MapSettingsData settings = MapSettingsData.Get();
            MapSceneData data = MapManager.Get()?.GetMapSceneData();

            if (zone != null && settings != null && settings.fog && data != null)
            {
                RenderTexture render_texture = GetTexture(zone.zone_id);

                //Precalculate values
                //float size_factor = 1f / settings.zone.GetSize().y;
                float factor_scale = viewer.GetFactorScale();
                Vector2 map_pos = viewer.GetCurrentPos();
                Vector2 pos_offset = Vector2.zero;
                float circle_size = 1f;
                float pos_mult = 1f;

                if (multi_resolution)
                {
                    circle_size = factor_scale * 0.5f;
                    pos_offset = map_pos * factor_scale * 0.5f * 0.5f;
                    pos_mult = factor_scale * 0.5f;
                }

                //setup render
                RenderTexture.active = render_texture;
                GL.Clear(true, true, fog_color);
                GL.Color(fog_color);

                GL.PushMatrix();
                GL.LoadOrtho();

                //Draw BG
                bg_mat.SetPass(0);
                GL.Begin(GL.QUADS);
                DrawRect();
                GL.End();

                //Draw circles
                fog_mat.SetPass(0);
                GL.Begin(GL.QUADS);

                //Render fog circles
                foreach (KeyValuePair<FogId, float> pair in data.fog_reveal)
                {
                    if (pair.Key.zone == zone.zone_id)
                    {
                        float size_factor = zone != null ? 1f / zone.GetSize().y : 1f;

                        //Position
                        Vector2 pos = MapTool.GetFogMapPos(pair.Key); //Return pos from -1 to 1
                        pos = (pos * pos_mult + Vector2.one) * 0.5f; //Convert to 0 to 1
                        pos = pos - pos_offset;

                        //Radius
                        float radius = pair.Value * size_factor * circle_size;
                        Vector2 radius_vect = new Vector2(radius, radius);
                        radius_vect.x = radius / aspect_ratio;

                        //Draw
                        DrawCircle(pos, radius_vect);
                    }
                }

                GL.End();
                GL.PopMatrix();

                RenderTexture.active = null;
                rendered = true;
            }
        }

        //pos should be between 0 and 1
        private void DrawCircle(Vector2 pos, Vector2 radius)
        {
            GL.TexCoord3(0.0f, 0.0f, 0f);
            GL.Vertex3(pos.x - radius.x, pos.y - radius.y, 0f);

            GL.TexCoord3(0f, 1f, 0f);
            GL.Vertex3(pos.x - radius.x, pos.y + radius.y, 0f);

            GL.TexCoord3(1f, 1f, 0f);
            GL.Vertex3(pos.x + radius.x, pos.y + radius.y, 0f);

            GL.TexCoord3(1f, 0f, 0f);
            GL.Vertex3(pos.x + radius.x, pos.y - radius.y, 0f);
        }

        private void DrawRect()
        {
            GL.TexCoord3(0.0f, 0.0f, 0f);
            GL.Vertex3(0f, 0f, 0f);

            GL.TexCoord3(0f, 1f, 0f);
            GL.Vertex3(0f, 1f, 0f);

            GL.TexCoord3(1f, 1f, 0f);
            GL.Vertex3(1f, 1f, 0f);

            GL.TexCoord3(1f, 0f, 0f);
            GL.Vertex3(1f, 0f, 0f);
        }

        public RenderTexture GetTexture(int id)
        {
            if (render_textures.ContainsKey(id))
                return render_textures[id];
            return null;
        }

        public Vector2Int GetTextureSize()
        {
            return texture_size;
        }

        public bool IsRendered()
        {
            return rendered;
        }
    }

}