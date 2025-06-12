terraform {
  cloud {
    organization = "M2Dev"

    workspaces {
      name = "pixup-dev"
    }
  }

  required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = "us-east-1"

  default_tags {
    tags = {
      Application = "Pixup"
      Environment = var.env_type
    }
  }
}

locals {
  is_prod = var.env_type == "prod"

  s3_origin_id = "s3origin"
}

resource "aws_s3_bucket" "content_bucket" {
  bucket = "${var.env_name}"

  # TODO: Add policies, etc
}

# resource "aws_s3_bucket_policy" "bucket_policy" {
#   bucket = aws_s3_bucket.content_bucket.id
#
#   policy = jsonencode({
#     Version = "2012-10-17"
#     Statement = [
#       {
#         Effect = "Allow"
#         Principal = {
#           Service = "cloudfront.amazonaws.com"
#         }
#         Action = "s3:GetObject"
#         Resource = "${aws_s3_bucket.content_bucket.arn}/*"
#         Condition = {
#           StringEquals = {
#             "AWS:SourceArn" = "${aws_cloudfront_distribution.cf_distribution.arn}"
#           }
#         }
#       }
#     ]
#   })
# }

# resource "aws_cloudfront_origin_access_control" "default" {
#   name = "${var.env_name}-s3-control"
#   description = "Allows access to from CF to the '${aws_s3_bucket.content_bucket.id}' bucket"
#   origin_access_control_origin_type =  "s3"
#   signing_behavior = "always"
#   signing_protocol = "sigv4"
# }
#
# resource "aws_cloudfront_distribution" "cf_distribution" {
#   origin {
#     domain_name = aws_s3_bucket.content_bucket.bucket_regional_domain_name
#     origin_access_control_id = aws_cloudfront_origin_access_control.default.id
#     origin_id = local.s3_origin_id
#   }
#
#   enabled = true
#   comment = "CF Distribution for ${var.env_name}"
#
#   default_cache_behavior {
#     allowed_methods = [ "GET", "HEAD", "OPTIONS" ]
#     cached_methods =  [ "GET", "HEAD" ]
#     target_origin_id = local.s3_origin_id
#
#     viewer_protocol_policy = "allow-all"
#     min_ttl = 0
#     default_ttl = 3600
#     max_ttl = 86400
#
#     forwarded_values {
#       query_string = true
#
#       cookies {
#         forward = "none"
#       }
#     }
#   }
#
#   restrictions {
#     geo_restriction {
#       restriction_type = "none"
#       # locations = [ "US", " ]
#     }
#   }
#
#   viewer_certificate {
#     cloudfront_default_certificate = true
#   }
# }
