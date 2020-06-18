using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using Kickstarter.Data.Models;

namespace Kickstarter.Data
{
    public partial class SQLDbContext : DbContext
    {
        public SQLDbContext()
        {
        }

        public SQLDbContext(DbContextOptions<SQLDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<FollowerToUser> FollowerToUser { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<PostToTag> PostToTag { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserCategoryExpert> UserCategoryExpert { get; set; }
        public virtual DbSet<UserTagExpert> UserTagExpert { get; set; }
        public virtual DbSet<UserVotes> UserVotes { get; set; }
        public virtual DbSet<Zone> Zone { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=kickstarter.co5dyjzk9lvp.us-east-1.rds.amazonaws.com;port=3306;database=kickstarter;uid=admin;pwd=password", x => x.ServerVersion("5.7.22-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.ZoneId)
                    .HasName("address_zone_id_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasColumnName("address_1")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address2)
                    .HasColumnName("address_2")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Postcode)
                    .IsRequired()
                    .HasColumnName("postcode")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ZoneId)
                    .HasColumnName("zone_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("address_zone_id");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.HasIndex(e => e.PostId)
                    .HasName("comment_post_id_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("comment_user_id_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.PostId)
                    .HasColumnName("post_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("comment_post_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comment_user_id");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<FollowerToUser>(entity =>
            {
                entity.HasKey(e => new { e.FollowerId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("follower_to_user");

                entity.HasIndex(e => e.FollowerId)
                    .HasName("follower_to_user_follower_id_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("follower_to_user_user_id_idx");

                entity.Property(e => e.FollowerId)
                    .HasColumnName("follower_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.FollowerToUserFollower)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("follower_to_user_follower_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FollowerToUserUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("follower_to_user_user_id");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("category_id_idx");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("post_user_id_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnName("state")
                    .HasColumnType("enum('PENDING','APPROVED','DELETED')")
                    .HasDefaultValueSql("'PENDING'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("post_category_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("post_user_id");
            });

            modelBuilder.Entity<PostToTag>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.TagId })
                    .HasName("PRIMARY");

                entity.ToTable("post_to_tag");

                entity.HasIndex(e => e.PostId)
                    .HasName("post_to_tag_post_id_idx");

                entity.HasIndex(e => e.TagId)
                    .HasName("post_to_tag_tag_id_idx");

                entity.Property(e => e.PostId)
                    .HasColumnName("post_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.TagId)
                    .HasColumnName("tag_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostToTag)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("post_to_tag_post_id");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PostToTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("post_to_tag_tag_id");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.NormalizedName)
                    .IsRequired()
                    .HasColumnName("normalized_name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.AddressId)
                    .HasName("user_address_id_idx");

                entity.HasIndex(e => e.Email)
                    .HasName("email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("username_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.AddressId)
                    .HasColumnName("address_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(96)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasColumnName("lastname")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Points)
                    .HasColumnName("points")
                    .HasColumnType("int(10) unsigned zerofill");

                entity.Property(e => e.Telephone)
                    .HasColumnName("telephone")
                    .HasColumnType("varchar(32)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_address_id");
            });

            modelBuilder.Entity<UserCategoryExpert>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CategoryId })
                    .HasName("PRIMARY");

                entity.ToTable("user_category_expert");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("user_category_expert_category_id_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_category_user_id_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.UserCategoryExpert)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_category_expert_category_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCategoryExpert)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_category_expert_user_id");
            });

            modelBuilder.Entity<UserTagExpert>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.TagId })
                    .HasName("PRIMARY");

                entity.ToTable("user_tag_expert");

                entity.HasIndex(e => e.TagId)
                    .HasName("user_tag_expert_tag_id_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_tag_expert_user_id_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.TagId)
                    .HasColumnName("tag_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.UserTagExpert)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_tag_expert_tag_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTagExpert)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_tag_expert_user_id");
            });

            modelBuilder.Entity<UserVotes>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PostId })
                    .HasName("PRIMARY");

                entity.ToTable("user_votes");

                entity.HasIndex(e => e.PostId)
                    .HasName("user_votes_post_id_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_votes_user_id_idx");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.PostId)
                    .HasColumnName("post_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Direction)
                    .HasColumnName("direction")
                    .HasColumnType("bit(1)");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.UserVotes)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("user_votes_post_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserVotes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_votes_user_id");
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("zone");

                entity.HasIndex(e => e.CountryId)
                    .HasName("zone_country_id_idx");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => new { e.CountryId, e.Name })
                    .HasName("zone_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CountryId)
                    .HasColumnName("country_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Zone)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("zone_country_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
