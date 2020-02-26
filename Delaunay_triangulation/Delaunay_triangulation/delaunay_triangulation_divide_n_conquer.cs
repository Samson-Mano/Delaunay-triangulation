using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delaunay_triangulation
{
    class delaunay_triangulation_divide_n_conquer
    {
        // References
        // Primitives for the manipulation of general subdivisions and the computation of Voronoi diagrams", ACM Transactions on Graphics, 4(2), 1985, 75–123
        // http://www.sccg.sk/~samuelcik/dgs/quad_edge.pdf

        // Dwyer, R.A., A faster divide-and-conquer algorithm for constructing
        // Delaunay triangulations.Algorithmica 2(2):137-151, 1987.
        // https://github.com/rexdwyer/DelaunayTriangulation/blob/master/common.c

        // http://www.neolithicsphere.com/geodesica/doc/quad_edge_overview.htm
        // https://cp-algorithms.com/geometry/delaunay.html
        // http://www.karlchenofhell.org/cppswp/lischinski.pdf
        // https://pure.mpg.de/rest/items/item_1819432_4/component/file_2599484/content

        // Lecture Notes on Delaunay Mesh Generation
        // Jonathan Richard Shewchuk
        // http://web.mit.edu/ehliu/Public/ProjectX/Summer2005/delnotes.pdf
        // Triangle: Engineering a 2D Quality Mesh Generator and Delaunay Triangulator
        // https://people.eecs.berkeley.edu/~jrs/papers/triangle.pdf
        // http://www.geom.uiuc.edu/~samuelp/del_project.html

        //___________________________________________________________________________________________________________________________________

        public void delaunay_start(List<Form1.planar_object_store.point2d> input_points,
                 ref List<Form1.planar_object_store.edge2d> output_edges,
                 ref List<Form1.planar_object_store.face2d> output_triangles,
                 ref List<Form1.planar_object_store.instance_tracker> output_edge_tracker)
        {
            // constructor for the main umberlla class
            // This class only takes  list of points as inputs
            // The input class can be modified as long as it can output pt.x & pt.y as a list



            // sort the point with x coordinate (y is a tie breaker)
            List<Form1.planar_object_store.point2d> sorted_pts = input_points.OrderBy(obj => obj.x).ThenBy(obj => obj.y).ToList();

            // vertex strore intialization
            List<Mesh_store.point_store> point_list = new List<Mesh_store.point_store>(); // initialize the vertex store delaunay pts

            // Transfer the input points to the delaunay pts (very in-efficient but I want the program to be versatille and easy to understand and implement)
            // revise this part to suit your requirements
            int id = 0; // id starts at zero 0
            foreach (Form1.planar_object_store.point2d inpt_pt in sorted_pts)
            {
                point_list.Add(new Mesh_store.point_store(id, inpt_pt.x, inpt_pt.y, inpt_pt)); //vert_type = true since its an user input delaunay points
                id++;
            }

            Mesh_store main_mesh = new Mesh_store(point_list, ref sorted_pts, ref output_edges, ref output_triangles, ref output_edge_tracker); // Main call
        }

        public class Mesh_store
        {
            //#### Output Variables #######
            private List<Form1.planar_object_store.point2d> local_input_points = new List<Form1.planar_object_store.point2d>();
            List<Form1.planar_object_store.edge2d> local_output_edges = new List<Form1.planar_object_store.edge2d>();
            List<Form1.planar_object_store.face2d> local_output_triangle = new List<Form1.planar_object_store.face2d>();
            List<Form1.planar_object_store.instance_tracker> local_history_tracker = new List<Form1.planar_object_store.instance_tracker>();
            //#####################################

            // Local Variables
            List<point_store> pt_list = new List<point_store>();
            List<edge_store> edge_list = new List<edge_store>();
            List<triangle_store> triangle_list = new List<triangle_store>();

            // unique id list
            List<int> unique_edgeid_list = new List<int>();
            List<int> unique_triangleid_list = new List<int>();

            public Mesh_store(List<point_store> input_vertices, ref List<Form1.planar_object_store.point2d> sorted_inpt_points, ref List<Form1.planar_object_store.edge2d> output_edges, ref List<Form1.planar_object_store.face2d> output_faces, ref List<Form1.planar_object_store.instance_tracker> output_history_tracker)
            {
                // Set the local input points to local variable
                local_input_points = sorted_inpt_points;

                // Intialize the outputs
                output_edges = new List<Form1.planar_object_store.edge2d>();  // Edges;
                output_faces = new List<Form1.planar_object_store.face2d>(); // Faces
                output_history_tracker = new List<Form1.planar_object_store.instance_tracker>(); // History tracker for animation

                // Transfer to Local point list
                pt_list = input_vertices;

                if (pt_list.Count > 3)// First call to delaunay divide and conquer
                {
                    int half_length = Convert.ToInt32(Math.Ceiling(pt_list.Count / 2.0f));
                    List<point_store> left_list = pt_list.GetRange(0, half_length); // extract the left list
                    List<point_store> right_list = pt_list.GetRange(half_length, pt_list.Count - half_length); // extract the right list
                    // ______________________________________________________________________________
                    delaunay_divide_n_conquer(left_list, right_list);// delaunay divide and conquer  |
                    // ______________________________________________________________________________|
                }
                else // If intial pt_list < 3 no need to divide and conquer
                {
                    if (pt_list.Count == 3)
                    {
                        add_triangle(pt_list[0], pt_list[1], pt_list[2]);
                    }
                    else // pt_list == 2
                    {
                        add_edge(pt_list[0], pt_list[1]);
                    }
                }

                // Results from local variables are transfered to output
                output_edges = local_output_edges;
                output_faces = local_output_triangle;
                output_history_tracker = local_history_tracker;
            }

            public void delaunay_divide_n_conquer(List<point_store> left_list, List<point_store> right_list)
            {
                // main divide and conquer recursive function

                // Left
                if (left_list.Count > 3)
                {
                    int half_length = Convert.ToInt32(Math.Ceiling(left_list.Count / 2.0f));
                    List<point_store> left_left_list = left_list.GetRange(0, half_length); // extract the left list
                    List<point_store> left_right_list = left_list.GetRange(half_length, left_list.Count - half_length); // extract the right list
                                                                                                                        // delaunay divide and conquer
                    delaunay_divide_n_conquer(left_left_list, left_right_list);
                }
                else
                {
                    if (left_list.Count == 3)
                    {
                        add_triangle(left_list[0], left_list[1], left_list[2]);
                    }
                    else // pts.count == 2
                    {
                        add_edge(left_list[0], left_list[1]);
                    }
                }

                // Right
                if (right_list.Count > 3)
                {
                    int half_length = Convert.ToInt32(Math.Ceiling(right_list.Count / 2.0f));
                    List<point_store> right_left_list = right_list.GetRange(0, half_length); // extract the left list
                    List<point_store> right_right_list = right_list.GetRange(half_length, right_list.Count - half_length); // extract the right list
                                                                                                                           // delaunay divide and conquer
                    delaunay_divide_n_conquer(right_left_list, right_right_list);
                }
                else
                {
                    if (right_list.Count == 3)
                    {
                        add_triangle(right_list[0], right_list[1], right_list[2]);
                    }
                    else // pts.count == 2
                    {
                        add_edge(right_list[0], right_list[1]);
                    }
                }

                edge_store baseLR = find_intial_baseLR(left_list, right_list);
                //Link LR edges
                Merge_LRedges(baseLR);
            }

            private void add_to_local_list(edge_store e, triangle_store t)
            {
                //____________________________________________________________________________________________________________________________________________________________________
                // !! Addition on main list here
                //Form1.planar_object_store.point2d spt = local_input_points.Find(obj => obj.Equals(new Form1.planar_object_store.point2d(e.start_pt.pt_id, e.start_pt.x, e.start_pt.y)));
                //Form1.planar_object_store.point2d ept = local_input_points.Find(obj => obj.Equals(new Form1.planar_object_store.point2d(-1, e.end_pt.x, e.end_pt.y)));
                Form1.planar_object_store.edge2d temp_edge = new Form1.planar_object_store.edge2d(e.edge_id, e.start_pt.get_parent_data_type, e.end_pt.get_parent_data_type);
                local_output_edges.Add(temp_edge);

                if (t != null)
                {
                    Form1.planar_object_store.face2d temp_face = new Form1.planar_object_store.face2d(t.tri_id, t.p1.get_parent_data_type, t.p2.get_parent_data_type, t.p3.get_parent_data_type);
                    local_output_triangle.Add(temp_face);
                }

                // #################################################################
                Form1.planar_object_store.instance_tracker temp_edge_tracker = new Form1.planar_object_store.instance_tracker();
                temp_edge_tracker.edge_list = new List<Form1.planar_object_store.edge2d>();
                temp_edge_tracker.edge_list.AddRange(local_output_edges);
                temp_edge_tracker.face_list = new List<Form1.planar_object_store.face2d>();
                temp_edge_tracker.face_list.AddRange(local_output_triangle);
                local_history_tracker.Add(temp_edge_tracker);
                // #################################################################
            }

            private void delete_from_local_list(edge_store e, int t1_id, int t2_id)
            {
                Form1.planar_object_store.edge2d temp_edge, temp_edge_sym;
                int rem_index;

                // edge e
                // !! removal on main list here
                temp_edge = new Form1.planar_object_store.edge2d(e.edge_id, e.start_pt.get_parent_data_type, e.end_pt.get_parent_data_type);
                temp_edge_sym = new Form1.planar_object_store.edge2d(e.edge_id, e.end_pt.get_parent_data_type, e.start_pt.get_parent_data_type);

                rem_index = local_output_edges.FindIndex(obj => obj.Equals(temp_edge) || obj.Equals(temp_edge_sym));

                if (rem_index != -1)
                {
                    local_output_edges.RemoveAt(rem_index); // !! Deletion on main list here

                    if (t1_id != -1 || t2_id != -1) // Check whether triangle need to be removed
                    {
                        if (t1_id < t2_id) // swap to remove in order
                        {
                            int temp = t1_id;
                            t1_id = t2_id;
                            t2_id = temp;
                        }

                        if (t1_id != -1)
                        {
                            int t1_index = local_output_triangle.FindIndex(obj => obj.face_id == triangle_list[t1_id].tri_id);
                            local_output_triangle.RemoveAt(t1_index);
                        }

                        if (t2_id != -1)
                        {
                            int t2_index = local_output_triangle.FindIndex(obj => obj.face_id == triangle_list[t2_id].tri_id);
                            local_output_triangle.RemoveAt(t2_index);
                        }
                    }


                    // #################################################################
                    Form1.planar_object_store.instance_tracker temp_edge_tracker = new Form1.planar_object_store.instance_tracker();
                    temp_edge_tracker.edge_list = new List<Form1.planar_object_store.edge2d>();
                    temp_edge_tracker.edge_list.AddRange(local_output_edges);
                    temp_edge_tracker.face_list = new List<Form1.planar_object_store.face2d>();
                    temp_edge_tracker.face_list.AddRange(local_output_triangle);
                    local_history_tracker.Add(temp_edge_tracker);
                    // #################################################################
                }
            }

            private void add_triangle(point_store p1, point_store p2, point_store p3)
            {
                // Add the first two edge
                add_edge(p1, p2);
                add_edge(p2, p3);

                // Check colinearity of p1, p2, p3
                if (is_collinear(p1, p2, p3) == false) // Add the third edge only when collinear is false
                {
                    add_edge_with_triangle(p3, p1); // Add third edge with triangle
                }
            }

            private void remove_triangle(int r_tri_id)
            {
                triangle_list[r_tri_id].e1.delete_triangle_id(triangle_list[r_tri_id].tri_id); // delete the triangle index in first edge
                triangle_list[r_tri_id].e2.delete_triangle_id(triangle_list[r_tri_id].tri_id); // delete the triangle index in secomd edge
                triangle_list[r_tri_id].e3.delete_triangle_id(triangle_list[r_tri_id].tri_id); // delete the triangle index in third edge

                unique_triangleid_list.Add(triangle_list[r_tri_id].tri_id);
                triangle_list.RemoveAt(r_tri_id); // Remove the triange
            }

            private void add_edge(point_store p1, point_store p2)
            {
                int edge_id = get_unique_edge_id();
                edge_store e = new edge_store(edge_id, p1, p2);

                // Update the points p1 & p2 -> edge id lists
                // Note points are never removed from the list so indexing is easier (pt_id remains the same)
                pt_list[p1.pt_id].add_sorted_edge(e);
                pt_list[p2.pt_id].add_sorted_edge(e);

                edge_list.Add(e); // Add the edge e to the list

                add_to_local_list(e, null);

            }

            private void add_edge_with_triangle(point_store p1, point_store p2)
            {
                int edge_id = get_unique_edge_id();
                edge_store e = new edge_store(edge_id, p1, p2);

                // Update the points p1 & p2 -> edge id lists
                // Note points are never removed from the list so indexing is easier (pt_id remains the same)
                pt_list[p1.pt_id].add_sorted_edge(e);
                pt_list[p2.pt_id].add_sorted_edge(e);

                // Now find the other two edges connected to this edge forming a triangle
                bool is_triangle_found = false;
                edge_store second_edge, third_edge;

                // clockwise search
                second_edge = e.end_pt.cc_vertical_edge(e);
                third_edge = second_edge.end_pt.cc_vertical_edge(second_edge);

                if (third_edge.end_pt.Equals(e.start_pt) == true)
                {
                    is_triangle_found = true;
                }
                else
                {
                    // counter clockwise search
                    second_edge = e.end_pt.cw_vertical_edge(e);
                    third_edge = second_edge.end_pt.cw_vertical_edge(second_edge);

                    if (third_edge.end_pt.Equals(e.start_pt) == true)
                    {
                        is_triangle_found = true;
                    }
                }
                edge_list.Add(e); // Add the edge e to the list

                if (is_triangle_found == true) // must found otherwise something went wrong
                {
                    int tri_id = get_unique_triangle_id();

                    int second_edge_index, third_edge_index;

                    second_edge_index = edge_list.FindIndex(obj => obj.edge_id == second_edge.edge_id);
                    third_edge_index = edge_list.FindIndex(obj => obj.edge_id == third_edge.edge_id);

                    edge_list[second_edge_index].add_triangle_id(tri_id); // got lazy.. need to develop more efficient method
                    edge_list[third_edge_index].add_triangle_id(tri_id); // got lazy.. need to develop more efficient method

                    // Update the edge triangle id
                    edge_list[edge_list.Count - 1].add_triangle_id(tri_id);

                    //Add the triangle
                    triangle_list.Add(new triangle_store(tri_id, e.start_pt, e.end_pt, second_edge.the_other_pt(e.end_pt)));

                    triangle_list[triangle_list.Count - 1].e1 = edge_list[third_edge_index];
                    triangle_list[triangle_list.Count - 1].e2 = edge_list[second_edge_index];
                    triangle_list[triangle_list.Count - 1].e3 = edge_list[edge_list.Count - 1];
                }


                add_to_local_list(e, is_triangle_found == true ? triangle_list[triangle_list.Count - 1] : null);
            }

            private void remove_edge(int r_edge_id)
            {
                int edge_list_index = edge_list.FindIndex(obj => obj.edge_id == r_edge_id);

                // Remove the edge ids in the points
                pt_list[edge_list[edge_list_index].start_pt.pt_id].delete_edge(edge_list[edge_list_index].edge_id);
                pt_list[edge_list[edge_list_index].end_pt.pt_id].delete_edge(edge_list[edge_list_index].edge_id);

                // Note no edges with two triangle (ie, triangle with either side) is removed, so finding two index is not efficient <- need improvement below
                // Find the triangle1 ids in the adjacent sides of this edge
                int tri_list_index1 = -1, tri_list_index2 = -1;

                tri_list_index1 = triangle_list.FindIndex(obj => obj.tri_id == edge_list[edge_list_index].get_first_tri_index);

                if (tri_list_index1 == -1)
                {
                    // Find the triangle2 (if any) ids in the adjacent sides of this edges
                    tri_list_index2 = triangle_list.FindIndex(obj => obj.tri_id == edge_list[edge_list_index].get_second_tri_index);
                }

                delete_from_local_list(edge_list[edge_list_index], tri_list_index1, tri_list_index2);

                // Remove the triangle1 ids in the adjacent sides of this edge
                if (tri_list_index1 != -1) // if -1 then not found
                {
                    remove_triangle(tri_list_index1); // Remove the triange
                }
                else if (tri_list_index2 != -1) // if -1 then not found // Remove the triangle2 (if any) ids in the adjacent sides of this edges
                {
                    remove_triangle(tri_list_index2); // Remove the triange
                }
                // update the unique edge id list to maintain a the edge ids
                unique_edgeid_list.Add(edge_list[edge_list_index].edge_id);
                // remove the edge
                edge_list.RemoveAt(edge_list_index);
            }

            private int get_unique_edge_id()
            {
                int edge_id;
                // get an unique edge id
                if (unique_edgeid_list.Count != 0)
                {
                    edge_id = unique_edgeid_list[0]; // retrive the edge id from the list which stores the id of deleted edges
                    unique_edgeid_list.RemoveAt(0); // remove that id from the unique edge id list
                }
                else
                {
                    edge_id = edge_list.Count;
                }
                return edge_id;
            }

            private int get_unique_triangle_id()
            {
                int tri_id;
                // get an unique triangle id
                if (unique_triangleid_list.Count != 0)
                {
                    tri_id = unique_triangleid_list[0]; // retrive the triangle id from the list which stores the id of deleted edges
                    unique_triangleid_list.RemoveAt(0); // remove that id from the unique triangle id list
                }
                else
                {
                    tri_id = triangle_list.Count;
                }
                return tri_id;
            }

            public edge_store find_intial_baseLR(List<point_store> left, List<point_store> right)
            {

                point_store left_end = left[left.Count - 1]; // left end // Colinear error fixed
                point_store right_end = right[0];// right end

                edge_store left_bot_edge = left_end.cw_vertical_edge(0); // First Vertical edge at clock wise direction at this point
                edge_store right_bot_edge = right_end.cc_vertical_edge(0); // First Vertical edge at counter clock wise direction at this point

                while (true)
                {
                    // Select the bottom most end by comparing the orientation with the other
                    if (leftof(right_end, left_bot_edge) == true) // check the right_end point and orientation of the left edge
                    {
                        left_end = left_bot_edge.the_other_pt(left_end); // Find the next point (whihc is the endpoint of the left edge)
                        left_bot_edge = left_end.cw_vertical_edge(0);
                    }
                    else if (rightof(left_end, right_bot_edge) == true) // check the left_end point and orientation of the right edge
                    {
                        right_end = right_bot_edge.the_other_pt(right_end);  // Find the next point (which is the endpoint of the right edge)
                        right_bot_edge = right_end.cc_vertical_edge(0);
                    }
                    else
                    {
                        break;
                    }
                }

                add_edge(left_end, right_end); // Add the base LR edge
                return edge_list[edge_list.Count - 1]; // return the last add item (which is the baseLR edge)
            }

            public void Merge_LRedges(edge_store baseLRedge)
            {
                edge_store baseLRedge_sym = new edge_store(baseLRedge.edge_id, baseLRedge.end_pt, baseLRedge.start_pt); // symmetry of baseLRedge
                edge_store lcand = baseLRedge.start_pt.cc_vertical_edge(baseLRedge);// left candidate
                edge_store rcand = baseLRedge.end_pt.cw_vertical_edge(baseLRedge_sym);// rigth candidate

                // Left side Remove operation
                if (leftof(lcand.end_pt, baseLRedge) == true) // if the left candidate end point is not leftof baseLRedge then top is reached with baseLRedge and the end point of leftcandidate will not lie inCircle
                {
                    edge_store lcand_next = baseLRedge.start_pt.cc_vertical_edge(lcand); // find the next candidate by counter clockwise cycle at baseLRedge start point with the current left candidate as qualifier
                    while (incircle(baseLRedge.start_pt, baseLRedge.end_pt, lcand.end_pt, lcand_next.end_pt) == true)
                    {
                        edge_store new_lcand = baseLRedge.start_pt.cc_vertical_edge(lcand); // find the next candidate by counter clockwise cycle at baseLRedge start point with the current left candidate as qualifier
                        remove_edge(lcand.edge_id);
                        lcand = new_lcand;
                        lcand_next = baseLRedge.start_pt.cc_vertical_edge(lcand);
                    }
                }

                // Right side Remove operation
                if (rightof(rcand.end_pt, baseLRedge_sym) == true) // if the right candidate end point is not rightof baseLRedge_sym then top is reached with baseLRedge and the end point of rightcandidate will not lie inCircle
                {
                    edge_store rcand_next = baseLRedge.end_pt.cw_vertical_edge(rcand); // find the next candidate by clockwise cycle at baseLRedge end point with the current right candidate as qualifier
                    while (incircle(baseLRedge_sym.end_pt, baseLRedge_sym.start_pt, rcand.end_pt, rcand_next.end_pt) == true)
                    {
                        edge_store new_rcand = baseLRedge.end_pt.cw_vertical_edge(rcand); // find the next candidate by clockwise cycle at baseLRedge end point with the current right candidate as qualifier
                        remove_edge(rcand.edge_id);
                        rcand = new_rcand;
                        rcand_next = baseLRedge.end_pt.cw_vertical_edge(rcand);
                    }
                }


                bool lvalid, rvalid;
                lvalid = leftof(lcand.end_pt, baseLRedge); // validity of left candidate
                rvalid = rightof(rcand.end_pt, baseLRedge_sym); // validity of right candidate

                // The next cross edge is to be connected to either lcand.end_pt or rcand.end_pt
                if (lvalid == true && rvalid == true) // both are valid, then choose the correct end with in-circle test
                {
                    if (incircle(baseLRedge.start_pt, baseLRedge.end_pt, lcand.end_pt, rcand.end_pt) == true) //right candidate end point lies inside in circle formed by left candidate
                    {
                        add_edge_with_triangle(baseLRedge.start_pt, rcand.end_pt); // so form the edge with right candidate end point
                    }
                    else // left candidate end point in cicle doesn't enclose right candidate end point
                    {
                        add_edge_with_triangle(lcand.end_pt, baseLRedge_sym.start_pt); // so form the edge with left candidate end point
                    }
                }
                else if (lvalid == true)
                {
                    add_edge_with_triangle(lcand.end_pt, baseLRedge_sym.start_pt);// Add cross edge base1 from lcand.end_pt to baseLRedge_sym.start_pt
                }
                else if (rvalid == true)
                {
                    add_edge_with_triangle(baseLRedge.start_pt, rcand.end_pt);// Add cross edge basel frombaseLRedge.start_pt to rcand.end_pt 
                }
                else
                {
                    // both lcand and rcand are making obtuse angle with baseLRedge, then baseLRedge is the upper common tangent
                    return; // end of recursion
                }


                edge_store new_baseLRedge = edge_list[edge_list.Count - 1]; // new baseLRedge is the last created edge
                Merge_LRedges(new_baseLRedge); // Recursion here
            }

            public class point_store
            {
                int _pt_id;
                double _x;
                double _y;
                private List<edge_store> _connected_edge_list = new List<edge_store>();
                Form1.planar_object_store.point2d store_parent_data;

                public point_store(int i_pt_id, double i_x, double i_y, Form1.planar_object_store.point2d i_inpt_as_point2d)
                {
                    this._pt_id = i_pt_id; // point id
                    this._x = i_x; // x
                    this._y = i_y; // y
                    this.store_parent_data = i_inpt_as_point2d;

                }

                public int pt_id
                {
                    get { return this._pt_id; }
                }

                public double x
                {
                    get { return this._x; }
                }

                public double y
                {
                    get { return this._y; }
                }

                public Form1.planar_object_store.point2d get_parent_data_type // very important to change the data type depends on the user original data type
                {
                    get { return this.store_parent_data; }
                }

                public edge_store cc_vertical_edge(int id) //counter clockwise vertical edge
                {
                    if (id > (this._connected_edge_list.Count - 1))
                        return null;

                    return this._connected_edge_list[id]; //0,1,2,.....
                }

                public edge_store cc_vertical_edge(edge_store with_edge) //counter clockwise vertical edge
                {
                    int index_of_next = this._connected_edge_list.FindIndex(obj => obj.edge_id == with_edge.edge_id);

                    if (index_of_next == -1) // object not found
                        return null; // this should never happen

                    index_of_next++; // next counter clockwise edge in the list
                    if (index_of_next == this._connected_edge_list.Count) // check if the index reached end
                        index_of_next = 0; //cycle back to zero

                    return cc_vertical_edge(index_of_next); //0,1,2,.....
                }

                public edge_store cw_vertical_edge(int id)
                {
                    if (id > (this._connected_edge_list.Count - 1)) // clockwise vertical edge
                        return null;

                    return this._connected_edge_list[this._connected_edge_list.Count - 1 - id]; //n-0,n-1,n-2,...
                }

                public edge_store cw_vertical_edge(edge_store with_edge) // clockwise vertical edge
                {
                    int index_of_next = this._connected_edge_list.FindIndex(obj => obj.edge_id == with_edge.edge_id);

                    if (index_of_next == -1) // object not found
                        return null; // this should never happen

                    index_of_next--; // next clockwise edge in the list
                    if (index_of_next == -1) // check if the index reached begining
                        index_of_next = this._connected_edge_list.Count - 1; //cycle back to end (reverse)

                    return cc_vertical_edge(index_of_next); //0,1,2,.....
                }

                public bool Equals(point_store other)
                {
                    return (this.x == other.x && this.y == other.y);
                }

                private edge_store get_edge_away_from_this_pt(edge_store the_edge)
                {
                    // This function returns the edge oriented from this point
                    point_store this_pt = new point_store(this._pt_id, this._x, this._y, this.store_parent_data);

                    if (the_edge.start_pt.Equals(this_pt) == false)
                    {

                        return new edge_store(the_edge.edge_id, this_pt, the_edge.the_other_pt(this_pt));
                    }
                    else
                    {
                        return the_edge;
                    }
                }

                public void add_sorted_edge(edge_store edge_to_add)
                {
                    // Add the edges as counter clock wise to the sorted list
                    //     this_pt
                    //      |\     
                    //      | \  
                    //      |  \ 
                    //      |   \
                    //      |    \
                    //      |     \
                    //      V      V
                    //  vertical   edge_0
                    //____________________________________________________________________________________

                    edge_to_add = get_edge_away_from_this_pt(edge_to_add); // Always add edge away from this point

                    if (this._connected_edge_list.Count == 0)
                    {
                        this._connected_edge_list.Add(edge_to_add);
                        return;
                    }

                    if (new edge_angle_comparer_vertical().Compare(this._connected_edge_list[this._connected_edge_list.Count - 1], edge_to_add) <= 0) // Equal to zero should not occur
                    {
                        this._connected_edge_list.Add(edge_to_add);
                        return;
                    }

                    if (new edge_angle_comparer_vertical().Compare(this._connected_edge_list[0], edge_to_add) >= 0) // Equal to zero should not occur
                    {
                        this._connected_edge_list.Insert(0, edge_to_add); // Insert at zero because all the other angles are higher
                        return;
                    }

                    // Uses a binary search algorithm to locate a specific element in the sorted List<edge_store>
                    int index = this._connected_edge_list.BinarySearch(edge_to_add, new edge_angle_comparer_vertical());
                    if (index < 0)
                        index = ~index; // Bitwise Complement operator is represented by ~.It is a unary operator, i.e.operates on only one operand.The ~ operator inverts each bits i.e.changes 1 to 0 and 0 to 1.
                    this._connected_edge_list.Insert(index, edge_to_add);
                }

                public void delete_edge(edge_store edge_to_delete)
                {
                    int edge_index = this._connected_edge_list.FindIndex(obj => obj.Equals(edge_to_delete));

                    if (edge_index != -1)
                    {
                        this._connected_edge_list.RemoveAt(edge_index);
                    }
                }

                public void delete_edge(int the_edge_id)
                {
                    int edge_index = this._connected_edge_list.FindIndex(obj => obj.edge_id == the_edge_id);

                    if (edge_index != -1)
                    {
                        this._connected_edge_list.RemoveAt(edge_index);
                    }
                }

            }

            private class edge_angle_comparer_vertical : IComparer<edge_store>
            {
                public int Compare(edge_store e1, edge_store e2)
                {
                    // if return is less than 0 (then e1 is less than e2)
                    // if return equals 0 (then e1 is equal to e2)
                    // if return is greater than 0 (then e1 is greater than e2)
                    edge_store vert_edge1, vert_edge2;
                    double angle_e1, angle_e2;

                    vert_edge1 = new edge_store(-1, e1.start_pt, new point_store(-1, e1.start_pt.x, e1.start_pt.y - 100, null));
                    angle_e1 = angle_between(vert_edge1, e1);

                    vert_edge2 = new edge_store(-1, e2.start_pt, new point_store(-1, e2.start_pt.x, e2.start_pt.y - 100, null));
                    angle_e2 = angle_between(vert_edge2, e2);

                    // A signed integer that indicates the relative values of x and y:
                    //  -If less than 0, x is less than y.
                    //  - If 0, x equals y.
                    //  - If greater than 0, x is greater than y.
                    if (angle_e1 < angle_e2)
                    {
                        return -1;
                    }
                    else if (angle_e1 > angle_e2)
                    {
                        return +1;
                    }
                    else
                    {
                        return 0; // Zero is not a case (never). If this line executes something went wrong!!
                    }
                }
            }

            public class edge_store
            {
                int _edge_id;
                point_store _start_pt;
                point_store _end_pt;
                int tri1_id;
                int tri2_id; // id of triangle 1 & 2

                public int edge_id
                {
                    get { return this._edge_id; }
                }

                public string get_str // Remove later
                {
                    get { return this._start_pt.pt_id.ToString() + " --> " + this._end_pt.pt_id.ToString(); }
                }

                public point_store start_pt
                {
                    get { return this._start_pt; }
                }

                public point_store end_pt
                {
                    get { return this._end_pt; }
                }

                public int get_first_tri_index
                {
                    get { return this.tri1_id; }
                }

                public int get_second_tri_index
                {
                    get { return this.tri2_id; }
                }

                public edge_store()
                {
                    // Empty constructor
                }

                public edge_store(int i_e_id, point_store s, point_store e)
                {
                    this._edge_id = i_e_id; // id of the edge
                    this._start_pt = s;
                    this._end_pt = e;
                    this.tri1_id = -1;
                    this.tri2_id = -1;
                }

                public void add_triangle_id(int t_id) // function to add the id of triangles attached to this edge (when new triangle added using this edge)
                {
                    // only two ids are stored
                    if (this.tri1_id == -1 || this.tri1_id == t_id)
                    {
                        this.tri1_id = t_id;
                    }
                    else
                    {
                        this.tri2_id = t_id;
                    }
                }

                public void delete_triangle_id(int t_id) // function to remove the id of triangles connected to this point (if the edge is removed)
                {
                    if (this.tri1_id == t_id)
                    {
                        this.tri1_id = -1;
                    }

                    if (this.tri2_id == t_id)
                    {
                        this.tri2_id = -1;
                    }
                }

                public bool Equals(edge_store other) // orientation is important
                {
                    return (this._start_pt.Equals(other.start_pt) && this._end_pt.Equals(other.end_pt));
                }

                public bool commutative_equal(edge_store other) // commutative equals check edges with same two points but different orientation
                {
                    return ((this._start_pt.Equals(other.start_pt) && this._end_pt.Equals(other.end_pt)) ||
                        (this._end_pt.Equals(other.start_pt) && this._start_pt.Equals(other.end_pt)));
                }

                public point_store the_other_pt(point_store this_pt)
                {
                    if (this_pt.Equals(start_pt) == true)
                        return end_pt;
                    else
                        return start_pt;
                }
            }

            public class triangle_store
            {
                int _tri_id;
                public edge_store e1, e2, e3; // Public variable
                point_store _p1, _p2, _p3;

                public point_store p1
                {
                    get { return this._p1; }
                }

                public point_store p2
                {
                    get { return this._p2; }
                }

                public point_store p3
                {
                    get { return this._p3; }
                }

                public int tri_id { get { return this._tri_id; } }

                public triangle_store(int i_tri_id, point_store i_p1, point_store i_p2, point_store i_p3)
                {
                    this._tri_id = i_tri_id;
                    this._p1 = i_p1;
                    this._p2 = i_p2;
                    this._p3 = i_p3;
                }

                public bool equal_edge(edge_store other_e)
                {
                    if (other_e.Equals(this.e1) == true || other_e.Equals(this.e2) == true || other_e.Equals(this.e3) == true)
                        return true;
                    else
                        return false;
                }

            }

            #region "Helper Static Functions"
            // key difference between a static and a non-static method is that static method belongs to a class while non-static method belongs to the instance
            public static double angle_between(edge_store with_edge, edge_store the_edge)
            {
                double v1_x, v1_y;
                double v2_x, v2_y;
                double normalzie;
                // vector with edge
                v1_x = with_edge.end_pt.x - with_edge.start_pt.x;
                v1_y = with_edge.end_pt.y - with_edge.start_pt.y;
                normalzie = Math.Sqrt(Math.Pow(v1_x, 2) + Math.Pow(v1_y, 2));

                v1_x = v1_x / normalzie;
                v1_y = v1_y / normalzie;

                // vector the edge
                v2_x = the_edge.end_pt.x - the_edge.start_pt.x;
                v2_y = the_edge.end_pt.y - the_edge.start_pt.y;
                normalzie = Math.Sqrt(Math.Pow(v2_x, 2) + Math.Pow(v2_y, 2));

                v2_x = v2_x / normalzie;
                v2_y = v2_y / normalzie;

                // sin and cos of the two vectors
                double sin = (v1_x * v2_y) - (v2_x * v1_y);
                double cos = (v1_x * v2_x) + (v1_y * v2_y);

                double angle = (Math.Atan2(sin, cos) / Math.PI) * 180f;
                if (angle <= 0) // there is no zero degree (zero degree = 360 degree) to avoid the vertical line mismatch
                    angle += 360f;

                return angle;
            }

            public static bool is_collinear(point_store a, point_store b, point_store c)
            {
                return ((((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x))) == 0);
            }

            private static bool ccw(point_store a, point_store b, point_store c)
            {
                // Computes | a.x a.y  1 |
                //          | b.x b.y  1 | > 0
                //          | c.x c.y  1 |
                return (((b.x - a.x) * (c.y - a.y)) - ((b.y - a.y) * (c.x - a.x))) > 0;
            }

            private static bool rightof(point_store x, edge_store e)
            {
                return ccw(x, e.end_pt, e.start_pt);
            }

            private static bool leftof(point_store x, edge_store e)
            {
                return ccw(x, e.start_pt, e.end_pt);
            }

            public bool incircle(point_store a, point_store b, point_store c, point_store d)
            {
                //Computes | a.x  a.y  a.x²+a.y²  1 |
                //         | b.x  b.y  b.x²+b.y²  1 | > 0
                //         | c.x  c.y  c.x²+c.y²  1 |
                //         | d.x  d.y  d.x²+d.y²  1 |
                // Return true is d is in the circumcircle of a, b, c
                // From Jon Shewchuk's "Fast Robust predicates for Computational geometry"
                double a1 = a.x - d.x;
                double a2 = a.y - d.y;

                double b1 = b.x - d.x;
                double b2 = b.y - d.y;

                double c1 = c.x - d.x;
                double c2 = c.y - d.y;

                double a3 = Math.Pow(a1, 2) + Math.Pow(a2, 2);
                double b3 = Math.Pow(b1, 2) + Math.Pow(b2, 2);
                double c3 = Math.Pow(c1, 2) + Math.Pow(c2, 2);

                double det = (a1 * b2 * c3 + a2 * b3 * c1 + a3 * b1 * c2) - (a3 * b2 * c1 + a1 * b3 * c2 + a2 * b1 * c3);

                return (det > 0); // Determinant greater than zero means inside the circle
            }
            #endregion
        }
    }
}