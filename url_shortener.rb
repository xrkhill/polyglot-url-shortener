#!/usr/bin/env ruby

require 'openssl'

class UrlShortener
  def shorten(url)
    # generate random bytes, and add process id and current time for more entropy
    num_bytes = 1024
    random_bytes = Random.new.bytes(num_bytes)
    pid = Process.pid
    time = Time.now.utc.to_i

    # combine url with random data
    combined = "#{url}#{random_bytes}#{pid}#{time}"

    # apply sha1 hash to combined data to get fixed number of bytes
    digest = OpenSSL::Digest::SHA1.new.digest(combined)

    # convert digest to integer
    integer = digest_to_int(digest)

    # convert to base 36 so we only return alpha numeric chars
    result = int_to_base_36(integer)

    # truncate value to seven chars so it will be short
    num_chars = 7
    return result[0..num_chars - 1]
  end

  private

  BASE = 36
  CHARS = "0123456789abcdefghijklmnopqrstuvwxyz"

  # Convert integer to base 36 (0-9, a-z)
  def int_to_base_36(integer)
    result = ""

    if (integer.negative?)
      integer = ~integer
    end

    while (integer > 0)
      result = CHARS[integer % BASE] + result
      integer /= BASE
    end

    return result
  end

  # Unpack digest into integers (64-bit signed, native endian (int64_t)).
  # Unpack returns an array of 64 bit integers, join.to_i overflows,
  # and Ruby automatically converts result into a Bignum.
  def digest_to_int(digest)
    digest.to_s.unpack("q*").join.to_i
  end
end

if __FILE__ == $0
  urls = [
    "http://www.google.com",
    "http://www.amazon.com",
    "http://www.netflix.com",
    "http://www.hulu.com",
    "http://www.linkedin.com",
    "http://www.facebook.com",
    "http://www.apple.com"
  ]

  shortener = UrlShortener.new

  urls.each do |url|
    puts "#{url} => http://cor.to/#{shortener.shorten(url)}"
  end
end
